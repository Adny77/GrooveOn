using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class SongService
        : BaseCRUDService<SongResponse, SongSearchObject, Song, SongUpsertRequest, SongUpsertRequest>,
          ISongService
    {
        private readonly GrooveOnDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGenreService _genreService;

        public SongService(
            GrooveOnDbContext context,
            IMapper mapper,
            IGenreService genreService)
            : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
            _genreService = genreService;
        }

        protected override IQueryable<Song> ApplyFilter(IQueryable<Song> query, SongSearchObject? search = null)
        {
            query = base.ApplyFilter(query, search);

            if (search == null)
                return query;

            if (search.ArtistId.HasValue)
            {
                query = query.Where(x => x.ArtistId == search.ArtistId.Value);
            }

            if (search.AlbumId.HasValue)
            {
                query = query.Where(x => x.AlbumId == search.AlbumId.Value);
            }

            if (search.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == search.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    (x.Artist != null && x.Artist.Name.ToLower().Contains(fts)) ||
                    (x.Album != null && x.Album.Title.ToLower().Contains(fts)));
            }

            return query;
        }

        protected override IQueryable<Song> AddInclude(IQueryable<Song> query, SongSearchObject search)
        {
            query = base.AddInclude(query, search);

            if (search.IncludeArtist == true)
            {
                query = query.Include(x => x.Artist);
            }

            if (search.IncludeAlbum == true)
            {
                query = query.Include(x => x.Album);
            }

            return query;
        }

        public async Task<SongDuplicateCheckResponse> CheckDuplicatesAsync(SongDuplicateCheckRequest request)
        {
            var ids = request.ExternalTrackIds
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (!ids.Any())
            {
                return new SongDuplicateCheckResponse();
            }

            var existingSongs = await _context.Songs
                .Include(x => x.Artist)
                .Include(x => x.Album)
                .Where(x => x.ExternalTrackId != null && ids.Contains(x.ExternalTrackId))
                .Select(x => new ExistingSongInfoResponse
                {
                    Id = x.Id,
                    ExternalTrackId = x.ExternalTrackId,
                    Title = x.Title,
                    ArtistName = x.Artist != null ? x.Artist.Name : "",
                    AlbumTitle = x.Album != null ? x.Album.Title : null,
                    CoverUrl = x.CoverUrl
                })
                .ToListAsync();

            var existingIds = existingSongs
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
                .Select(x => x.ExternalTrackId!)
                .ToHashSet();

            var missingIds = ids
                .Where(x => !existingIds.Contains(x))
                .ToList();

            return new SongDuplicateCheckResponse
            {
                ExistingSongs = existingSongs,
                MissingExternalTrackIds = missingIds
            };
        }

        public async Task<SongBulkInsertResponse> BulkInsertDeezerSongsAsync(SongBulkInsertRequest request)
        {
            var items = request.Songs
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
                .GroupBy(x => x.ExternalTrackId)
                .Select(g => g.First())
                .ToList();

            if (!items.Any())
            {
                return new SongBulkInsertResponse();
            }

            var externalIds = items
                .Select(x => x.ExternalTrackId)
                .ToList();

            var existingIds = await _context.Songs
                .Where(x => x.ExternalTrackId != null && externalIds.Contains(x.ExternalTrackId))
                .Select(x => x.ExternalTrackId!)
                .ToListAsync();

            var existingSet = existingIds.ToHashSet();

            var toInsert = items
                .Where(x => !existingSet.Contains(x.ExternalTrackId))
                .ToList();

            var savedSongIds = new List<int>();

            foreach (var item in toInsert)
            {
                var artist = await ResolveArtistAsync(item);
                var album = await ResolveAlbumAsync(item, artist);

                if (album != null && item.Genres.Any())
                {
                    await SaveAlbumGenresAsync(album.Id, item.Genres);
                }

                var entity = new Song
                {
                    ExternalTrackId = item.ExternalTrackId,
                    Source = item.Source,
                    Title = item.Title,
                    ArtistId = artist.Id,
                    AlbumId = album?.Id,
                    DurationSeconds = item.DurationSeconds,
                    PreviewUrl = item.PreviewUrl,
                    CoverUrl = item.CoverUrl,
                    ReleaseDate = item.ReleaseDate,
                    LastSyncedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Songs.Add(entity);
                await _context.SaveChangesAsync();

                savedSongIds.Add(entity.Id);
            }

            return new SongBulkInsertResponse
            {
                SavedCount = savedSongIds.Count,
                SavedSongIds = savedSongIds
            };
        }

        private async Task<Artist> ResolveArtistAsync(SongUpsertRequest item)
        {
            Artist? artist = null;

            if (!string.IsNullOrWhiteSpace(item.ExternalArtistId))
            {
                artist = await _context.Artists
                    .FirstOrDefaultAsync(x => x.ExternalArtistId == item.ExternalArtistId);
            }

            if (artist == null && !string.IsNullOrWhiteSpace(item.ArtistName))
            {
                artist = await _context.Artists
                    .FirstOrDefaultAsync(x => x.Name == item.ArtistName);
            }

            if (artist == null)
            {
                artist = new Artist
                {
                    ExternalArtistId = item.ExternalArtistId,
                    Source = string.IsNullOrWhiteSpace(item.Source) ? "Deezer" : item.Source,
                    Name = item.ArtistName,
                    Picture = item.ArtistPicture
                };

                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }
            else
            {
                var changed = false;

                if (string.IsNullOrWhiteSpace(artist.ExternalArtistId) && !string.IsNullOrWhiteSpace(item.ExternalArtistId))
                {
                    artist.ExternalArtistId = item.ExternalArtistId;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(artist.Picture) && !string.IsNullOrWhiteSpace(item.ArtistPicture))
                {
                    artist.Picture = item.ArtistPicture;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(artist.Source) && !string.IsNullOrWhiteSpace(item.Source))
                {
                    artist.Source = item.Source;
                    changed = true;
                }

                if (changed)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return artist;
        }

        private async Task<Album?> ResolveAlbumAsync(SongUpsertRequest item, Artist artist)
        {
            if (string.IsNullOrWhiteSpace(item.AlbumTitle) && string.IsNullOrWhiteSpace(item.ExternalAlbumId))
            {
                return null;
            }

            Album? album = null;

            if (!string.IsNullOrWhiteSpace(item.ExternalAlbumId))
            {
                album = await _context.Albums
                    .FirstOrDefaultAsync(x => x.ExternalAlbumId == item.ExternalAlbumId);
            }

            if (album == null && !string.IsNullOrWhiteSpace(item.AlbumTitle))
            {
                album = await _context.Albums
                    .FirstOrDefaultAsync(x => x.Title == item.AlbumTitle && x.ArtistId == artist.Id);
            }

            if (album == null)
            {
                album = new Album
                {
                    ExternalAlbumId = item.ExternalAlbumId,
                    Source = string.IsNullOrWhiteSpace(item.Source) ? "Deezer" : item.Source,
                    Title = item.AlbumTitle ?? item.Title,
                    ArtistId = artist.Id,
                    CoverUrl = item.CoverUrl,
                    ReleaseDate = item.ReleaseDate
                };

                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
            }
            else
            {
                var changed = false;

                if (string.IsNullOrWhiteSpace(album.ExternalAlbumId) && !string.IsNullOrWhiteSpace(item.ExternalAlbumId))
                {
                    album.ExternalAlbumId = item.ExternalAlbumId;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.Source) && !string.IsNullOrWhiteSpace(item.Source))
                {
                    album.Source = item.Source;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.CoverUrl) && !string.IsNullOrWhiteSpace(item.CoverUrl))
                {
                    album.CoverUrl = item.CoverUrl;
                    changed = true;
                }

                if (!album.ReleaseDate.HasValue && item.ReleaseDate.HasValue)
                {
                    album.ReleaseDate = item.ReleaseDate;
                    changed = true;
                }

                if (changed)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return album;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var song = await _context.Songs
                .Include(x => x.Album)
                    .ThenInclude(a => a.AlbumGenres)
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (song == null)
                throw new InvalidOperationException("Pjesma nije pronađena.");

            var albumId = song.AlbumId;
            var artistId = song.ArtistId;

            var playHistories = await _context.PlayHistories
                .Where(x => x.SongId == song.Id)
                .ToListAsync();

            if (playHistories.Any())
                _context.PlayHistories.RemoveRange(playHistories);

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            if (albumId.HasValue)
            {
                var albumSongsCount = await _context.Songs.CountAsync(x => x.AlbumId == albumId.Value);

                if (albumSongsCount == 0)
                {
                    var album = await _context.Albums
                        .Include(x => x.AlbumGenres)
                        .FirstOrDefaultAsync(x => x.Id == albumId.Value);

                    if (album != null)
                    {
                        var genreIds = album.AlbumGenres.Select(x => x.GenreId).Distinct().ToList();

                        if (album.AlbumGenres.Any())
                            _context.AlbumGenres.RemoveRange(album.AlbumGenres);

                        _context.Albums.Remove(album);
                        await _context.SaveChangesAsync();

                        foreach (var genreId in genreIds)
                        {
                            var genreStillUsed = await _context.AlbumGenres
                                .AnyAsync(x => x.GenreId == genreId);

                            if (!genreStillUsed)
                            {
                                var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == genreId);
                                if (genre != null)
                                    _context.Genres.Remove(genre);
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
            }

            var artistStillHasSongs = await _context.Songs.AnyAsync(x => x.ArtistId == artistId);
            if (!artistStillHasSongs)
            {
                var artist = await _context.Artists.FirstOrDefaultAsync(x => x.Id == artistId);
                if (artist != null)
                {
                    _context.Artists.Remove(artist);
                    await _context.SaveChangesAsync();
                }
            }

            await transaction.CommitAsync();
            return true;
        }

        private async Task SaveAlbumGenresAsync(int albumId, List<GenreUpsertRequest> genres)
        {
            if (genres == null || !genres.Any())
                return;

            foreach (var item in genres
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalGenreId) && !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => x.ExternalGenreId)
                .Select(g => g.First()))
            {
                var genre = await _context.Genres
                    .FirstOrDefaultAsync(x => x.ExternalGenreId == item.ExternalGenreId);

                if (genre == null)
                {
                    genre = new Genre
                    {
                        ExternalGenreId = item.ExternalGenreId,
                        Source = string.IsNullOrWhiteSpace(item.Source) ? "Deezer" : item.Source,
                        Name = item.Name
                    };

                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();
                }
                else if (string.IsNullOrWhiteSpace(genre.Name))
                {
                    genre.Name = item.Name;
                    await _context.SaveChangesAsync();
                }

                var exists = await _context.AlbumGenres
                    .AnyAsync(x => x.AlbumId == albumId && x.GenreId == genre.Id);

                if (!exists)
                {
                    _context.AlbumGenres.Add(new AlbumGenre
                    {
                        AlbumId = albumId,
                        GenreId = genre.Id
                    });

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
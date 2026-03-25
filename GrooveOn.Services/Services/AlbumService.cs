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
    public class AlbumService
        : BaseCRUDService<AlbumResponse, AlbumSearchObject, Album, AlbumUpsertRequest, AlbumUpsertRequest>,
          IAlbumService
    {
        private readonly GrooveOnDbContext _context;

        public AlbumService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
        }

        protected override IQueryable<Album> ApplyFilter(IQueryable<Album> query, AlbumSearchObject search)
        {
            return query
                .Include(x => x.Artist)
                .Include(x => x.Songs)
                .Include(x => x.AlbumGenres)
                    .ThenInclude(x => x.Genre);
        }

        protected override IQueryable<Album> AddInclude(IQueryable<Album> query, AlbumSearchObject search)
        {
            query = base.ApplyFilter(query, search);

            if (!string.IsNullOrWhiteSpace(search?.FTS))
            {
                var fts = search.FTS.Trim().ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    (x.Artist != null && x.Artist.Name.ToLower().Contains(fts))
                );
            }

            return query;
        }

        public async Task<AlbumPreviewResponse> PreviewDeezerAlbumAsync(AlbumUpsertRequest request)
        {
            var albumAlreadyExists = await _context.Albums
                .AnyAsync(x =>
                    x.ExternalAlbumId != null &&
                    x.ExternalAlbumId == request.ExternalAlbumId
                );

            var externalTrackIds = request.Tracks
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
                .Select(x => x.ExternalTrackId)
                .Distinct()
                .ToList();

            var existingTrackIds = await _context.Songs
                .Where(x =>
                    x.ExternalTrackId != null &&
                    externalTrackIds.Contains(x.ExternalTrackId))
                .Select(x => x.ExternalTrackId!)
                .ToListAsync();

            var existingTrackSet = existingTrackIds.ToHashSet();

            var tracks = request.Tracks
                .Select(x => new ExistingAlbumTrackResponse
                {
                    ExternalTrackId = x.ExternalTrackId,
                    Title = x.Title,
                    AlreadyExists = existingTrackSet.Contains(x.ExternalTrackId)
                })
                .ToList();

            return new AlbumPreviewResponse
            {
                AlbumAlreadyExists = albumAlreadyExists,
                Tracks = tracks,
                ExistingTracksCount = tracks.Count(x => x.AlreadyExists),
                NewTracksCount = tracks.Count(x => !x.AlreadyExists)
            };
        }

        public async Task<AlbumSaveResponse> SaveDeezerAlbumAsync(AlbumUpsertRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ExternalAlbumId))
                throw new Exception("ExternalAlbumId is required.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new Exception("Album title is required.");

            if (string.IsNullOrWhiteSpace(request.ArtistName))
                throw new Exception("Artist name is required.");

            var artist = await ResolveArtistAsync(request);
            var album = await ResolveAlbumAsync(request, artist);

            await SaveAlbumGenresAsync(album.Id, request.Genres);

            var externalTrackIds = request.Tracks
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
                .Select(x => x.ExternalTrackId)
                .Distinct()
                .ToList();

            var existingSongs = await _context.Songs
                .Where(x =>
                    x.ExternalTrackId != null &&
                    externalTrackIds.Contains(x.ExternalTrackId))
                .ToListAsync();

            var existingSongMap = existingSongs
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
                .ToDictionary(x => x.ExternalTrackId!, x => x);

            var existingTracksCount = existingSongMap.Count;
            var savedTracksCount = 0;

            foreach (var track in request.Tracks)
            {
                if (string.IsNullOrWhiteSpace(track.ExternalTrackId))
                    continue;

                if (existingSongMap.TryGetValue(track.ExternalTrackId, out var existingSong))
                {
                    var changed = false;

                    if (existingSong.AlbumId == null)
                    {
                        existingSong.AlbumId = album.Id;
                        changed = true;
                    }

                    if (existingSong.ArtistId != artist.Id)
                    {
                        existingSong.ArtistId = artist.Id;
                        changed = true;
                    }

                    if (string.IsNullOrWhiteSpace(existingSong.CoverUrl) &&
                        !string.IsNullOrWhiteSpace(track.CoverUrl ?? request.CoverUrl))
                    {
                        existingSong.CoverUrl = track.CoverUrl ?? request.CoverUrl;
                        changed = true;
                    }

                    if (existingSong.ReleaseDate == null &&
                        (track.ReleaseDate.HasValue || request.ReleaseDate.HasValue))
                    {
                        existingSong.ReleaseDate = track.ReleaseDate ?? request.ReleaseDate;
                        changed = true;
                    }

                    if (string.IsNullOrWhiteSpace(existingSong.PreviewUrl) &&
                        !string.IsNullOrWhiteSpace(track.PreviewUrl))
                    {
                        existingSong.PreviewUrl = track.PreviewUrl;
                        changed = true;
                    }

                    existingSong.LastSyncedAt = DateTime.UtcNow;

                    if (changed)
                    {
                        _context.Songs.Update(existingSong);
                    }
                }
                else
                {
                    var entity = new Song
                    {
                        ExternalTrackId = track.ExternalTrackId,
                        Source = string.IsNullOrWhiteSpace(track.Source) ? "Deezer" : track.Source,
                        Title = track.Title,
                        ArtistId = artist.Id,
                        AlbumId = album.Id,
                        DurationSeconds = track.DurationSeconds,
                        PreviewUrl = track.PreviewUrl,
                        CoverUrl = track.CoverUrl ?? request.CoverUrl,
                        ReleaseDate = track.ReleaseDate ?? request.ReleaseDate,
                        IsActive = true,
                        LastSyncedAt = DateTime.UtcNow,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Songs.Add(entity);
                    savedTracksCount++;
                }
            }

            await _context.SaveChangesAsync();

            return new AlbumSaveResponse
            {
                AlbumId = album.Id,
                AlbumCreated = !existingSongMap.Any() || album.CreatedAt.Date == DateTime.UtcNow.Date,
                SavedTracksCount = savedTracksCount,
                ExistingTracksCount = existingTracksCount
            };
        }

        private async Task<Artist> ResolveArtistAsync(AlbumUpsertRequest request)
        {
            Artist? artist = null;

            if (!string.IsNullOrWhiteSpace(request.ExternalArtistId))
            {
                artist = await _context.Artists
                    .FirstOrDefaultAsync(x => x.ExternalArtistId == request.ExternalArtistId);
            }

            if (artist == null)
            {
                artist = await _context.Artists
                    .FirstOrDefaultAsync(x => x.Name == request.ArtistName);
            }

            if (artist == null)
            {
                artist = new Artist
                {
                    ExternalArtistId = request.ExternalArtistId,
                    Source = string.IsNullOrWhiteSpace(request.Source) ? "Deezer" : request.Source,
                    Name = request.ArtistName,
                    Picture = null,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }
            else
            {
                var changed = false;

                if (string.IsNullOrWhiteSpace(artist.ExternalArtistId) &&
                    !string.IsNullOrWhiteSpace(request.ExternalArtistId))
                {
                    artist.ExternalArtistId = request.ExternalArtistId;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(artist.Source) &&
                    !string.IsNullOrWhiteSpace(request.Source))
                {
                    artist.Source = request.Source;
                    changed = true;
                }

                if (changed)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return artist;
        }

        private async Task<Album> ResolveAlbumAsync(AlbumUpsertRequest request, Artist artist)
        {
            Album? album = null;

            if (!string.IsNullOrWhiteSpace(request.ExternalAlbumId))
            {
                album = await _context.Albums
                    .Include(x => x.Songs)
                    .Include(x => x.AlbumGenres)
                    .FirstOrDefaultAsync(x => x.ExternalAlbumId == request.ExternalAlbumId);
            }

            if (album == null)
            {
                album = await _context.Albums
                    .Include(x => x.Songs)
                    .Include(x => x.AlbumGenres)
                    .FirstOrDefaultAsync(x =>
                        x.Title == request.Title &&
                        x.ArtistId == artist.Id);
            }

            if (album == null)
            {
                album = new Album
                {
                    ExternalAlbumId = request.ExternalAlbumId,
                    Source = string.IsNullOrWhiteSpace(request.Source) ? "Deezer" : request.Source,
                    Title = request.Title,
                    ArtistId = artist.Id,
                    ReleaseDate = request.ReleaseDate,
                    CoverUrl = request.CoverUrl,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
            }
            else
            {
                var changed = false;

                if (string.IsNullOrWhiteSpace(album.ExternalAlbumId) &&
                    !string.IsNullOrWhiteSpace(request.ExternalAlbumId))
                {
                    album.ExternalAlbumId = request.ExternalAlbumId;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.Source) &&
                    !string.IsNullOrWhiteSpace(request.Source))
                {
                    album.Source = request.Source;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.CoverUrl) &&
                    !string.IsNullOrWhiteSpace(request.CoverUrl))
                {
                    album.CoverUrl = request.CoverUrl;
                    changed = true;
                }

                if (!album.ReleaseDate.HasValue && request.ReleaseDate.HasValue)
                {
                    album.ReleaseDate = request.ReleaseDate;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.Description) &&
                    !string.IsNullOrWhiteSpace(request.Description))
                {
                    album.Description = request.Description;
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

            var album = await _context.Albums
                .Include(x => x.Songs)
                .Include(x => x.AlbumGenres)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (album == null)
                throw new InvalidOperationException("Album nije pronađen.");

            var songIds = album.Songs.Select(x => x.Id).ToList();
            var artistIds = album.Songs.Select(x => x.ArtistId).Distinct().ToList();
            var genreIds = album.AlbumGenres.Select(x => x.GenreId).Distinct().ToList();

            if (songIds.Any())
            {
                var playHistories = await _context.PlayHistories
                    .Where(x => songIds.Contains(x.SongId))
                    .ToListAsync();

                if (playHistories.Any())
                    _context.PlayHistories.RemoveRange(playHistories);

                _context.Songs.RemoveRange(album.Songs);
            }

            if (album.AlbumGenres.Any())
                _context.AlbumGenres.RemoveRange(album.AlbumGenres);

            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();

            foreach (var genreId in genreIds)
            {
                var genreStillUsed = await _context.AlbumGenres.AnyAsync(x => x.GenreId == genreId);
                if (!genreStillUsed)
                {
                    var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == genreId);
                    if (genre != null)
                        _context.Genres.Remove(genre);
                }
            }

            foreach (var artistId in artistIds)
            {
                var artistStillUsed = await _context.Songs.AnyAsync(x => x.ArtistId == artistId);
                if (!artistStillUsed)
                {
                    var artist = await _context.Artists.FirstOrDefaultAsync(x => x.Id == artistId);
                    if (artist != null)
                        _context.Artists.Remove(artist);
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return true;
        }

        private async Task SaveAlbumGenresAsync(int albumId, List<GenreUpsertRequest> genres)
        {
            if (genres == null || !genres.Any())
                return;

            var cleanGenres = genres
                .Where(x =>
                    !string.IsNullOrWhiteSpace(x.ExternalGenreId) &&
                    !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => x.ExternalGenreId)
                .Select(g => g.First())
                .ToList();

            if (!cleanGenres.Any())
                return;

            foreach (var item in cleanGenres)
            {
                var genre = await _context.Genres
                    .FirstOrDefaultAsync(x => x.ExternalGenreId == item.ExternalGenreId);

                if (genre == null)
                {
                    genre = new Genre
                    {
                        ExternalGenreId = item.ExternalGenreId,
                        Source = string.IsNullOrWhiteSpace(item.Source) ? "Deezer" : item.Source,
                        Name = item.Name,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Genres.Add(genre);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var changed = false;

                    if (string.IsNullOrWhiteSpace(genre.Name) &&
                        !string.IsNullOrWhiteSpace(item.Name))
                    {
                        genre.Name = item.Name;
                        changed = true;
                    }

                    if (string.IsNullOrWhiteSpace(genre.Source) &&
                        !string.IsNullOrWhiteSpace(item.Source))
                    {
                        genre.Source = item.Source;
                        changed = true;
                    }

                    if (changed)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                var exists = await _context.AlbumGenres.AnyAsync(x =>
                    x.AlbumId == albumId &&
                    x.GenreId == genre.Id
                );

                if (!exists)
                {
                    _context.AlbumGenres.Add(new AlbumGenre
                    {
                        AlbumId = albumId,
                        GenreId = genre.Id,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
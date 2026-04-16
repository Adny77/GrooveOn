using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class SongService
        : BaseCRUDService<SongResponse, SongSearchObject, Song, SongUpsertRequest, SongUpsertRequest>,
          ISongService
    {
        private readonly IMusicResolveService _musicResolveService;
        private readonly IAlbumGenreService _albumGenreService;
        private readonly IPlayHistoryService _playHistoryService;
        private readonly IArtistService _artistService;
        private readonly IGenreService _genreService;

        public SongService(
            GrooveOnDbContext context,
            IMapper mapper,
            IMusicResolveService musicResolveService,
            IAlbumGenreService albumGenreService,
            IGenreService genreService,
            IPlayHistoryService playHistoryService,
            IArtistService arstisService)
            : base(context, mapper)
        {
            _musicResolveService = musicResolveService;
            _albumGenreService = albumGenreService;
            _genreService = genreService;
            _artistService = arstisService;
            _playHistoryService = playHistoryService;
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
                var artist = await _musicResolveService.ResolveArtistAsync(new ResolveArtistRequest
                {
                    ExternalArtistId = item.ExternalArtistId,
                    ArtistName = item.ArtistName,
                    Source = item.Source,
                    ArtistPicture = item.ArtistPicture
                });

                var album = await _musicResolveService.ResolveAlbumAsync(new ResolveAlbumRequest
                {
                    ExternalAlbumId = item.ExternalAlbumId,
                    Title = item.AlbumTitle ?? item.Title,
                    Source = item.Source,
                    CoverUrl = item.CoverUrl,
                    ReleaseDate = item.ReleaseDate,
                    AllowNull = true,
                    IncludeDetails = false
                }, artist);

                if (album != null && item.Genres.Any())
                {
                    await _albumGenreService.SaveAlbumGenresAsync(album.Id, item.Genres);
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
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Songs.Add(entity);
                savedSongIds.Add(entity.Id);
            }

            await _context.SaveChangesAsync();

            return new SongBulkInsertResponse
            {
                SavedCount = savedSongIds.Count,
                SavedSongIds = savedSongIds
            };
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var song = await _context.Songs
                .Include(x => x.Album)
                    .ThenInclude(x => x.AlbumGenres)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (song == null)
                throw new NotFoundException("Song not found");

            var albumId = song.AlbumId;
            var artistId = song.ArtistId;

            await _playHistoryService.DeleteBySongIdsAsync(new List<int> { song.Id });

            _context.Songs.Remove(song);

            if (albumId.HasValue)
            {
                var albumSongIdsToIgnore = new List<int> { song.Id };

                var albumStillHasSongs = await _context.Songs
                    .AnyAsync(x => x.AlbumId == albumId.Value && x.Id != song.Id);

                if (!albumStillHasSongs)
                {
                    var album = await _context.Albums
                        .Include(x => x.AlbumGenres)
                        .FirstOrDefaultAsync(x => x.Id == albumId.Value);

                    if (album != null)
                    {
                        var genreIds = album.AlbumGenres
                            .Select(x => x.GenreId)
                            .Distinct()
                            .ToList();

                        await _albumGenreService.DeleteByAlbumIdAsync(album.Id);
                        _context.Albums.Remove(album);

                        await _genreService.DeleteUnusedGenresAsync(genreIds, albumIdToIgnore: album.Id);
                    }
                }
            }

            await _artistService.DeleteUnusedArtistsAsync(
                new List<int> { artistId },
                songIdsToIgnore: new List<int> { song.Id });

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
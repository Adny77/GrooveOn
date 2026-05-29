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

        public async Task<List<SongResponse>> GetRecommendedForUserAsync(int userId, int take = 4)
        {
            if (take <= 0)
                take = 4;

            var listenedSongIds = await _context.PlayHistories
                .Where(x => x.UserId == userId)
                .Select(x => x.SongId)
                .Distinct()
                .ToListAsync();

            if (!listenedSongIds.Any())
            {
                return await GetColdStartRecommendedAsync(take);
            }

            var listenedArtistIds = await _context.Songs
                .Where(x => listenedSongIds.Contains(x.Id))
                .Select(x => x.ArtistId)
                .Distinct()
                .ToListAsync();

            var listenedGenreIds = await _context.Songs
                .Where(x => listenedSongIds.Contains(x.Id) && x.Album != null)
                .SelectMany(x => x.Album!.AlbumGenres)
                .Select(x => x.GenreId)
                .Distinct()
                .ToListAsync();

            var recommendedCandidates = await _context.Songs
                .Include(x => x.Artist)
                .Include(x => x.Album)
                    .ThenInclude(x => x.AlbumGenres)
                .Where(x =>
                    x.IsActive &&
                    !listenedSongIds.Contains(x.Id) &&
                    (
                        listenedArtistIds.Contains(x.ArtistId) ||
                        x.Album!.AlbumGenres.Any(ag => listenedGenreIds.Contains(ag.GenreId))
                    ))
                .OrderByDescending(x => listenedArtistIds.Contains(x.ArtistId) ? 2 : 0)
                .ThenByDescending(x =>
                    x.Album!.AlbumGenres.Count(ag => listenedGenreIds.Contains(ag.GenreId)))
                .Take(50)
                .ToListAsync();

            var artistMatchIds = recommendedCandidates
                .Where(x => listenedArtistIds.Contains(x.ArtistId))
                .Select(x => x.Id)
                .ToHashSet();

            var recommended = recommendedCandidates
                .GroupBy(x => x.ArtistId)
                .Select(g => g.First())
                .Take(take)
                .ToList();

            var popularIds = new HashSet<int>();

            if (recommended.Count < take)
            {
                var existingSongIds = recommended.Select(x => x.Id).ToList();
                var existingArtistIds = recommended.Select(x => x.ArtistId).ToList();

                var fallbackCandidates = await _context.PlayHistories
                    .Where(x =>
                        !listenedSongIds.Contains(x.SongId) &&
                        !existingSongIds.Contains(x.SongId))
                    .GroupBy(x => x.SongId)
                    .Select(g => new
                    {
                        SongId = g.Key,
                        PlayCount = g.Count()
                    })
                    .OrderByDescending(x => x.PlayCount)
                    .Take(100)
                    .Join(
                        _context.Songs
                            .Include(x => x.Artist)
                            .Include(x => x.Album),
                        x => x.SongId,
                        s => s.Id,
                        (x, s) => s
                    )
                    .Where(x =>
                        x.IsActive &&
                        !existingArtistIds.Contains(x.ArtistId))
                    .ToListAsync();

                var fallback = fallbackCandidates
                    .GroupBy(x => x.ArtistId)
                    .Select(g => g.First())
                    .Take(take - recommended.Count)
                    .ToList();

                foreach (var s in fallback) popularIds.Add(s.Id);
                recommended.AddRange(fallback);
            }

            var extraIds = new HashSet<int>();

            if (recommended.Count < take)
            {
                var existingSongIds = recommended.Select(x => x.Id).ToList();
                var existingArtistIds = recommended.Select(x => x.ArtistId).ToList();

                var extraSongs = await _context.Songs
                    .Include(x => x.Artist)
                    .Include(x => x.Album)
                    .Where(x =>
                        x.IsActive &&
                        !listenedSongIds.Contains(x.Id) &&
                        !existingSongIds.Contains(x.Id) &&
                        !existingArtistIds.Contains(x.ArtistId))
                    .OrderBy(x => x.Title)
                    .Take(take - recommended.Count)
                    .ToListAsync();

                foreach (var s in extraSongs) extraIds.Add(s.Id);
                recommended.AddRange(extraSongs);
            }

            return recommended
                .Take(take)
                .Select(s =>
                {
                    var r = MapToResponse(s);
                    r.WhyRecommended = artistMatchIds.Contains(s.Id)
                        ? "Because you listen to this artist"
                        : popularIds.Contains(s.Id)
                            ? "Popular right now"
                            : extraIds.Contains(s.Id)
                                ? "Discover new music"
                                : "Based on genres you enjoy";
                    return r;
                })
                .ToList();
        }

        private async Task<List<SongResponse>> GetColdStartRecommendedAsync(int take)
        {
            var topSongs = await _context.PlayHistories
                .GroupBy(x => x.SongId)
                .Select(g => new
                {
                    SongId = g.Key,
                    PlayCount = g.Count()
                })
                .OrderByDescending(x => x.PlayCount)
                .Take(take)
                .Join(
                    _context.Songs
                        .Include(x => x.Artist)
                        .Include(x => x.Album),
                    x => x.SongId,
                    s => s.Id,
                    (x, s) => s
                )
                .ToListAsync();

            if (topSongs.Count < take)
            {
                var existingIds = topSongs.Select(x => x.Id).ToList();

                var extraSongs = await _context.Songs
                    .Include(x => x.Artist)
                    .Include(x => x.Album)
                    .Where(x => x.IsActive && !existingIds.Contains(x.Id))
                    .OrderBy(x => x.Title)
                    .Take(take - topSongs.Count)
                    .ToListAsync();

                topSongs.AddRange(extraSongs);
            }

            return topSongs
                .Take(take)
                .Select(MapToResponse)
                .ToList();
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

            var insertedEntities = new List<Song>();

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
                insertedEntities.Add(entity);
            }

            await _context.SaveChangesAsync();

            return new SongBulkInsertResponse
            {
                SavedCount = insertedEntities.Count,
                SavedSongIds = insertedEntities.Select(x => x.Id).ToList()
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
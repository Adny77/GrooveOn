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
    public class AlbumService
        : BaseCRUDService<AlbumResponse, AlbumSearchObject, Album, AlbumUpsertRequest, AlbumUpsertRequest>,
          IAlbumService
    {
        private readonly IMusicResolveService _musicResolveService;
        private readonly IAlbumGenreService _albumGenreService;
        private readonly IPlayHistoryService _playHistoryService;
        private readonly IArtistService _artistService;
        private readonly IGenreService _genreService;

        public AlbumService(
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

            if (search.ArtistId.HasValue)
            {
                query = query.Where(x => x.Artist.Id == search.ArtistId);
            }

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

        protected override AlbumResponse MapToResponse(Album entity)
        {
            var response = _mapper.Map<AlbumResponse>(entity);
            response.SongCount = entity.Songs?.Count ?? 0;
            return response;
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
                throw new UserException("ExternalAlbumId is required.");

            if (string.IsNullOrWhiteSpace(request.Title))
                throw new UserException("Album title is required.");

            if (string.IsNullOrWhiteSpace(request.ArtistName))
                throw new UserException("Artist name is required.");

            var artist = await _musicResolveService.ResolveArtistAsync(new ResolveArtistRequest
            {
                ExternalArtistId = request.ExternalArtistId,
                ArtistName = request.ArtistName,
                Source = request.Source,
                ArtistPicture = null
            });

            var album = await _musicResolveService.ResolveAlbumAsync(new ResolveAlbumRequest
            {
                ExternalAlbumId = request.ExternalAlbumId,
                Title = request.Title,
                Source = request.Source,
                CoverUrl = request.CoverUrl,
                ReleaseDate = request.ReleaseDate,
                Description = request.Description,
                AllowNull = false,
                IncludeDetails = true
            }, artist);

            if (album == null)
                throw new InvalidOperationException("Album not able to create");

            await _albumGenreService.SaveAlbumGenresAsync(album.Id, request.Genres);

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

        public override async Task<bool> DeleteAsync(int id)
        {
            var album = await _context.Albums
                .Include(x => x.Songs)
                .Include(x => x.AlbumGenres)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (album == null)
                throw new NotFoundException("Album not found");

            var songIds = album.Songs
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            var artistIds = album.Songs
                .Select(x => x.ArtistId)
                .Distinct()
                .ToList();

            var genreIds = album.AlbumGenres
                .Select(x => x.GenreId)
                .Distinct()
                .ToList();

            if (songIds.Any())
            {
                await _playHistoryService.DeleteBySongIdsAsync(songIds);
                _context.Songs.RemoveRange(album.Songs);
            }

            await _albumGenreService.DeleteByAlbumIdAsync(album.Id);
            _context.Albums.Remove(album);

            await _genreService.DeleteUnusedGenresAsync(genreIds, albumIdToIgnore: album.Id);
            await _artistService.DeleteUnusedArtistsAsync(artistIds, songIdsToIgnore: songIds);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
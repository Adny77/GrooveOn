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

        public SongService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
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
                var artist = await _context.Artists
                    .FirstOrDefaultAsync(x => x.Name == item.ArtistName);

                if (artist == null)
                {
                    artist = new Artist
                    {
                        Name = item.ArtistName
                    };
                    _context.Artists.Add(artist);
                    await _context.SaveChangesAsync();
                }

                Album? album = null;
                if (!string.IsNullOrWhiteSpace(item.AlbumTitle))
                {
                    album = await _context.Albums
                        .FirstOrDefaultAsync(x => x.Title == item.AlbumTitle && x.ArtistId == artist.Id);

                    if (album == null)
                    {
                        album = new Album
                        {
                            Title = item.AlbumTitle!,
                            ArtistId = artist.Id,
                            CoverUrl = item.CoverUrl,
                            ReleaseDate = item.ReleaseDate
                        };

                        _context.Albums.Add(album);
                        await _context.SaveChangesAsync();
                    }
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
    }
}
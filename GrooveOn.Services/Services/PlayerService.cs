using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using GrooveOn.Services.Services;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class PlayerService
        : BaseCRUDService<PlayerResponse, PlayerSearchObject, Player, PlayerUpsertRequest, PlayerUpsertRequest>,
          IPlayerService
    {
        private readonly ISongService _songService;
        private readonly IPlaylistSongService _playlistSongService;

        public PlayerService(
            GrooveOnDbContext context,
            IMapper mapper,
            ISongService songService,
            IPlaylistSongService playlistSongService)
            : base(context, mapper)
        {
            _songService = songService;
            _playlistSongService = playlistSongService;
        }

        public async Task<PlayerResponse> PlayRandomMusicAsync(PlayerUpsertRequest request)
        {
            ValidatePlayerRequest(request);

            if (request.Purpose == "RandomMusic")
                return await StartRandomQueueAsync(request);

            else if (request.Purpose == "RandomMusicArtist")
                return await StartRandomQueueAsync(request);

            else if (request.Purpose == "AlbumList")
                return await StartAlbumQueueAsync(request);

            else if (request.Purpose == "PlaylistList")
                return await StartPlaylistQueueAsync(request);

            throw new UserException("Unsupported purpose.");
        }

        public async Task<PlayerResponse> PlayNextRandomMusicAsync(PlayerUpsertRequest request)
        {
            ValidatePlayerRequest(request);

            if (request.Purpose == "RandomMusic" || request.Purpose == "RandomMusicArtist")
                return await PlayNextRandomQueueAsync(request);

            else if (request.Purpose == "AlbumList" || request.Purpose == "PlaylistList")
                return await PlayNextAlbumQueueAsync(request);

            throw new UserException("Unsupported purpose.");
        }

        public async Task<PlayerResponse> PlayPreviousRandomMusicAsync(PlayerUpsertRequest request)
        {
            ValidatePlayerRequest(request);

            if (request.Purpose == "RandomMusic" || request.Purpose == "RandomMusicArtist")
                return await PlayPreviousRandomQueueAsync(request);

            else if (request.Purpose == "AlbumList" || request.Purpose == "PlaylistList")
                return await PlayPreviousAlbumQueueAsync(request);

            throw new UserException("Unsupported purpose.");
        }


        private async Task<PlayerResponse> StartRandomQueueAsync(PlayerUpsertRequest request)
        {
            var clickedSong = await _context.Set<Song>()
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Id == request.SongId);

            if (clickedSong == null)
                throw new UserException("Song was not found.");

            await ClearUserQueueAsync(request.UserId);

            var playerItem = new Player
            {
                UserId = request.UserId,
                SongId = clickedSong.Id,
                Purpose = request.Purpose,
                OrderIndex = 1
            };

            _context.Set<Player>().Add(playerItem);
            _context.Set<PlayHistory>().Add(new PlayHistory { UserId = request.UserId, SongId = clickedSong.Id, PlayedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            return MapToPlayerResponse(playerItem, clickedSong, false, true);
        }


        private async Task<PlayerResponse> StartAlbumQueueAsync(PlayerUpsertRequest request)
        {
            var clickedSong = await _context.Set<Song>()
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Id == request.SongId);

            if (clickedSong == null)
                throw new UserException("Song was not found.");

            if (clickedSong.AlbumId == null)
                throw new UserException("Pjesma ne pripada albumu.");

            var search = new SongSearchObject
            {
                AlbumId = clickedSong.AlbumId,
                Page = 0,
                PageSize = 1000,
                IncludeTotalCount = true
            };

            var result = await _songService.GetAsync(search);

            var orderedIds = result.Items.Select(x => x.Id).ToList();

            var songs = await _context.Set<Song>()
                .Include(x => x.Artist)
                .Where(x => orderedIds.Contains(x.Id) && x.ExternalTrackId != null)
                .ToListAsync();

            var orderedSongs = orderedIds
                .Select(id => songs.FirstOrDefault(x => x.Id == id))
                .Where(x => x != null)
                .DistinctBy(x => x!.Id)
                .ToList();

            await ClearUserQueueAsync(request.UserId);

            var queue = orderedSongs
                .Select((song, i) => new Player
                {
                    UserId = request.UserId,
                    SongId = song!.Id,
                    Purpose = request.Purpose,
                    OrderIndex = i + 1
                }).ToList();

            _context.AddRange(queue);
            _context.Set<PlayHistory>().Add(new PlayHistory { UserId = request.UserId, SongId = request.SongId, PlayedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var current = queue.First(x => x.SongId == request.SongId);
            var currentSong = orderedSongs.First(x => x!.Id == request.SongId);

            return MapToPlayerResponse(
                current,
                currentSong!,
                current.OrderIndex > 1,
                current.OrderIndex < queue.Count
            );
        }


        private async Task<PlayerResponse> StartPlaylistQueueAsync(PlayerUpsertRequest request)
        {
            var clickedSong = await _context.Set<Song>()
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Id == request.SongId);

            if (clickedSong == null)
                throw new UserException("Song not found.");

            if (request.PlaylistId == null)
                throw new UserException("PlaylistId is required.");

            var exists = await _context.Set<PlaylistSong>()
                .AnyAsync(x =>
                    x.PlaylistId == request.PlaylistId &&
                    x.SongId == request.SongId);

            if (!exists)
                throw new UserException("Song does not belong to this playlist.");

            var playlistSongs = await _context.Set<PlaylistSong>()
                .Where(x => x.PlaylistId == request.PlaylistId)
                .OrderBy(x => x.Id)
                .ToListAsync();

            var songIds = playlistSongs.Select(x => x.SongId).ToList();

            var songs = await _context.Set<Song>()
                .Include(x => x.Artist)
                .Where(x => songIds.Contains(x.Id) && x.ExternalTrackId != null)
                .ToListAsync();

            var orderedSongs = songIds
                .Select(id => songs.FirstOrDefault(x => x.Id == id))
                .Where(x => x != null)
                .DistinctBy(x => x!.Id)
                .ToList();

            await ClearUserQueueAsync(request.UserId);

            var queue = orderedSongs
                .Select((song, index) => new Player
                {
                    UserId = request.UserId,
                    SongId = song!.Id,
                    Purpose = request.Purpose,
                    OrderIndex = index + 1
                })
                .ToList();

            _context.AddRange(queue);
            _context.Set<PlayHistory>().Add(new PlayHistory { UserId = request.UserId, SongId = request.SongId, PlayedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            var current = queue.First(x => x.SongId == request.SongId);
            var currentSong = orderedSongs.First(x => x!.Id == request.SongId);

            return MapToPlayerResponse(
                current,
                currentSong!,
                current.OrderIndex > 1,
                current.OrderIndex < queue.Count
            );
        }


        private async Task<PlayerResponse> PlayNextRandomQueueAsync(PlayerUpsertRequest request)
        {
            var current = await GetCurrentQueueItemAsync(request);

            var nextIndex = current.OrderIndex + 1;

            var next = await _context.Set<Player>()
                .Include(x => x.Song).ThenInclude(x => x.Artist)
                .FirstOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.OrderIndex == nextIndex);

            if (next != null)
            {
                await RecordPlayHistoryAsync(request.UserId, next.Song.Id);
                return MapToPlayerResponse(next, next.Song, next.OrderIndex > 1, true);
            }

            var existingSongIds = await _context.Set<Player>()
                .Where(x => x.UserId == request.UserId && x.Purpose == request.Purpose)
                .Select(x => x.SongId!.Value)
                .ToListAsync();

            var randomSong = await GetRandomSongAsync(request, existingSongIds);

            var newItem = new Player
            {
                UserId = request.UserId,
                SongId = randomSong.Id,
                Purpose = request.Purpose,
                OrderIndex = nextIndex
            };

            _context.Add(newItem);
            _context.Set<PlayHistory>().Add(new PlayHistory { UserId = request.UserId, SongId = randomSong.Id, PlayedAt = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            return MapToPlayerResponse(newItem, randomSong, true, true);
        }

        private async Task<PlayerResponse> PlayNextAlbumQueueAsync(PlayerUpsertRequest request)
        {
            var current = await GetCurrentQueueItemAsync(request);

            var total = await _context.Set<Player>()
                .CountAsync(x => x.UserId == request.UserId && x.Purpose == request.Purpose);

            if (current.OrderIndex >= total)
                throw new UserException("There is no next song.");

            var next = await _context.Set<Player>()
                .Include(x => x.Song).ThenInclude(x => x.Artist)
                .FirstAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.OrderIndex == current.OrderIndex + 1);

            await RecordPlayHistoryAsync(request.UserId, next.Song.Id);
            return MapToPlayerResponse(
                next,
                next.Song,
                next.OrderIndex > 1,
                next.OrderIndex < total
            );
        }

        private async Task<PlayerResponse> PlayPreviousRandomQueueAsync(PlayerUpsertRequest request)
        {
            var current = await GetCurrentQueueItemAsync(request);

            if (current.OrderIndex <= 1)
            {
                var song = await _context.Set<Song>()
                    .Include(x => x.Artist)
                    .FirstAsync(x => x.Id == current.SongId);

                await RecordPlayHistoryAsync(request.UserId, song.Id);
                return MapToPlayerResponse(current, song, false, true);
            }

            var previous = await _context.Set<Player>()
                .Include(x => x.Song).ThenInclude(x => x.Artist)
                .FirstAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.OrderIndex == current.OrderIndex - 1);

            await RecordPlayHistoryAsync(request.UserId, previous.Song.Id);
            return MapToPlayerResponse(previous, previous.Song, previous.OrderIndex > 1, true);
        }

        private async Task<PlayerResponse> PlayPreviousAlbumQueueAsync(PlayerUpsertRequest request)
        {
            var current = await GetCurrentQueueItemAsync(request);

            if (current.OrderIndex <= 1)
                throw new UserException("There is no previous song.");

            var total = await _context.Set<Player>()
                .CountAsync(x => x.UserId == request.UserId && x.Purpose == request.Purpose);

            var previous = await _context.Set<Player>()
                .Include(x => x.Song).ThenInclude(x => x.Artist)
                .FirstAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.OrderIndex == current.OrderIndex - 1);

            await RecordPlayHistoryAsync(request.UserId, previous.Song.Id);
            return MapToPlayerResponse(
                previous,
                previous.Song,
                previous.OrderIndex > 1,
                previous.OrderIndex < total
            );
        }


        protected override async Task BeforeInsert(Player entity, PlayerUpsertRequest request)
        {
            var duplicate = await _context.Set<Player>()
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.SongId == request.SongId);

            if (duplicate)
                throw new UserException("This song already exists in the queue for this purpose.");

            await base.BeforeInsert(entity, request);
        }

        private void ValidatePlayerRequest(PlayerUpsertRequest request)
        {
            if (request.UserId <= 0)
                throw new UserException("UserId is required.");

            if (request.SongId <= 0)
                throw new UserException("SongId is required.");

            var allowed = new[]
            {
                "RandomMusic",
                "RandomMusicArtist",
                "AlbumList",
                "PlaylistList"
            };

            if (!allowed.Contains(request.Purpose))
                throw new UserException("Unsupported purpose.");
        }

        private async Task<Player> GetCurrentQueueItemAsync(PlayerUpsertRequest request)
        {
            var item = await _context.Set<Player>()
                .FirstOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.SongId == request.SongId &&
                    x.Purpose == request.Purpose);

            if (item == null)
                throw new UserException("Current song was not found.");

            return item;
        }

        private async Task<Song> GetRandomSongAsync(PlayerUpsertRequest request, ICollection<int>? excludeSongIds = null)
        {
            var query = _context.Set<Song>()
                .Where(x => x.ExternalTrackId != null);

            if (request.Purpose == "RandomMusicArtist")
                query = query.Where(x => x.ArtistId == request.ArtistId);

            if (excludeSongIds != null && excludeSongIds.Count > 0)
                query = query.Where(x => !excludeSongIds.Contains(x.Id));

            var count = await query.CountAsync();

            if (count == 0)
            {
                // All unique songs exhausted — fall back without exclusion
                query = _context.Set<Song>().Where(x => x.ExternalTrackId != null);
                if (request.Purpose == "RandomMusicArtist")
                    query = query.Where(x => x.ArtistId == request.ArtistId);
                count = await query.CountAsync();
            }

            if (count == 0)
                throw new UserException("No songs available.");

            var skip = new Random().Next(count);

            return await query
                .Include(x => x.Artist)
                .Skip(skip)
                .FirstAsync();
        }

        private async Task ClearUserQueueAsync(int userId)
        {
            var old = await _context.Set<Player>()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            _context.RemoveRange(old);
        }

        private PlayerResponse MapToPlayerResponse(Player p, Song s, bool prev, bool next)
        {
            return new PlayerResponse
            {
                Id = p.Id,
                SongId = s.Id,
                Title = s.Title,
                CoverUrl = s.CoverUrl,
                ArtistName = s.Artist.Name,
                Duration = s.DurationSeconds.Value,
                PreviewUrl = s.PreviewUrl,
                ExternalTrackId = s.ExternalTrackId,
                HasPrevious = prev,
                HasNext = next
            };
        }

        private async Task RecordPlayHistoryAsync(int userId, int songId)
        {
            _context.Set<PlayHistory>().Add(new PlayHistory
            {
                UserId = userId,
                SongId = songId,
                PlayedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }
}

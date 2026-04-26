using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using GrooveOn.Services.Services;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services
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

            throw new UserException("Nepodržan purpose.");
        }

        public async Task<PlayerResponse> PlayNextRandomMusicAsync(PlayerUpsertRequest request)
        {
            ValidatePlayerRequest(request);

            if (request.Purpose == "RandomMusic" || request.Purpose == "RandomMusicArtist")
                return await PlayNextRandomQueueAsync(request);

            else if (request.Purpose == "AlbumList" || request.Purpose == "PlaylistList")
                return await PlayNextAlbumQueueAsync(request);

            throw new UserException("Nepodržan purpose.");
        }

        public async Task<PlayerResponse> PlayPreviousRandomMusicAsync(PlayerUpsertRequest request)
        {
            ValidatePlayerRequest(request);

            if (request.Purpose == "RandomMusic" || request.Purpose == "RandomMusicArtist")
                return await PlayPreviousRandomQueueAsync(request);

            else if (request.Purpose == "AlbumList" || request.Purpose == "PlaylistList")
                return await PlayPreviousAlbumQueueAsync(request);

            throw new UserException("Nepodržan purpose.");
        }

        // ================= RANDOM =================

        private async Task<PlayerResponse> StartRandomQueueAsync(PlayerUpsertRequest request)
        {
            var clickedSong = await _context.Set<Song>()
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Id == request.SongId);

            if (clickedSong == null)
                throw new UserException("Pjesma nije pronađena.");

            await ClearUserQueueAsync(request.UserId);

            var playerItem = new Player
            {
                UserId = request.UserId,
                SongId = clickedSong.Id,
                Purpose = request.Purpose,
                OrderIndex = 1
            };

            _context.Set<Player>().Add(playerItem);
            await _context.SaveChangesAsync();

            return MapToPlayerResponse(playerItem, clickedSong, false, true);
        }

        // ================= ALBUM =================

        private async Task<PlayerResponse> StartAlbumQueueAsync(PlayerUpsertRequest request)
        {
            var clickedSong = await _context.Set<Song>()
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Id == request.SongId);

            if (clickedSong == null)
                throw new UserException("Pjesma nije pronađena.");

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

        // ================= PLAYLIST =================

        private async Task<PlayerResponse> StartPlaylistQueueAsync(PlayerUpsertRequest request)
        {
            var clickedSong = await _context.Set<Song>()
                .Include(x => x.Artist)
                .FirstOrDefaultAsync(x => x.Id == request.SongId);

            if (clickedSong == null)
                throw new UserException("Pjesma nije pronađena.");

            var playlistSong = await _context.Set<PlaylistSong>()
                .FirstOrDefaultAsync(x => x.SongId == request.SongId);

            if (playlistSong == null)
                throw new UserException("Pjesma ne pripada playlisti.");

            var search = new PlaylistSongSearchObject
            {
                PlaylistId = playlistSong.PlaylistId,
                Page = 0,
                PageSize = 1000,
                IncludeTotalCount = true
            };

            var result = await _playlistSongService.GetAsync(search);

            var orderedIds = result.Items.Select(x => x.SongId).ToList();

            var songs = await _context.Set<Song>()
                .Include(x => x.Artist)
                .Where(x => orderedIds.Contains(x.Id) && x.ExternalTrackId != null)
                .ToListAsync();

            var orderedSongs = orderedIds
                .Select(id => songs.FirstOrDefault(x => x.Id == id))
                .Where(x => x != null)
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

        // ================= NEXT / PREVIOUS =================

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
                return MapToPlayerResponse(next, next.Song, next.OrderIndex > 1, true);

            var randomSong = await GetRandomSongAsync(request);

            var newItem = new Player
            {
                UserId = request.UserId,
                SongId = randomSong.Id,
                Purpose = request.Purpose,
                OrderIndex = nextIndex
            };

            _context.Add(newItem);
            await _context.SaveChangesAsync();

            return MapToPlayerResponse(newItem, randomSong, true, true);
        }

        private async Task<PlayerResponse> PlayNextAlbumQueueAsync(PlayerUpsertRequest request)
        {
            var current = await GetCurrentQueueItemAsync(request);

            var total = await _context.Set<Player>()
                .CountAsync(x => x.UserId == request.UserId && x.Purpose == request.Purpose);

            if (current.OrderIndex >= total)
                throw new UserException("Nema sljedeće pjesme.");

            var next = await _context.Set<Player>()
                .Include(x => x.Song).ThenInclude(x => x.Artist)
                .FirstAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.OrderIndex == current.OrderIndex + 1);

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

                return MapToPlayerResponse(current, song, false, true);
            }

            var previous = await _context.Set<Player>()
                .Include(x => x.Song).ThenInclude(x => x.Artist)
                .FirstAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.OrderIndex == current.OrderIndex - 1);

            return MapToPlayerResponse(previous, previous.Song, previous.OrderIndex > 1, true);
        }

        private async Task<PlayerResponse> PlayPreviousAlbumQueueAsync(PlayerUpsertRequest request)
        {
            var current = await GetCurrentQueueItemAsync(request);

            if (current.OrderIndex <= 1)
                throw new UserException("Nema prethodne pjesme.");

            var total = await _context.Set<Player>()
                .CountAsync(x => x.UserId == request.UserId && x.Purpose == request.Purpose);

            var previous = await _context.Set<Player>()
                .Include(x => x.Song).ThenInclude(x => x.Artist)
                .FirstAsync(x =>
                    x.UserId == request.UserId &&
                    x.Purpose == request.Purpose &&
                    x.OrderIndex == current.OrderIndex - 1);

            return MapToPlayerResponse(
                previous,
                previous.Song,
                previous.OrderIndex > 1,
                previous.OrderIndex < total
            );
        }

        // ================= HELPERS =================

        private void ValidatePlayerRequest(PlayerUpsertRequest request)
        {
            if (request.UserId <= 0)
                throw new UserException("UserId je obavezan.");

            if (request.SongId <= 0)
                throw new UserException("SongId je obavezan.");

            var allowed = new[]
            {
                "RandomMusic",
                "RandomMusicArtist",
                "AlbumList",
                "PlaylistList"
            };

            if (!allowed.Contains(request.Purpose))
                throw new UserException("Nepodržan purpose.");
        }

        private async Task<Player> GetCurrentQueueItemAsync(PlayerUpsertRequest request)
        {
            var item = await _context.Set<Player>()
                .FirstOrDefaultAsync(x =>
                    x.UserId == request.UserId &&
                    x.SongId == request.SongId &&
                    x.Purpose == request.Purpose);

            if (item == null)
                throw new UserException("Trenutna pjesma nije pronađena.");

            return item;
        }

        private async Task<Song> GetRandomSongAsync(PlayerUpsertRequest request)
        {
            var query = _context.Set<Song>()
                .Include(x => x.Artist)
                .Where(x => x.ExternalTrackId != null);

            if (request.Purpose == "RandomMusicArtist")
                query = query.Where(x => x.ArtistId == request.ArtistId);

            var songs = await query.ToListAsync();

            var rnd = new Random();
            return songs[rnd.Next(songs.Count)];
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
                ExternalTrackId = s.ExternalTrackId,
                HasPrevious = prev,
                HasNext = next
            };
        }
    }
}
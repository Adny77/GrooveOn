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
    public class PlaylistSongService
        : BaseCRUDService<PlaylistSongResponse, PlaylistSongSearchObject, PlaylistSong, PlaylistSongUpsertRequest, PlaylistSongUpsertRequest>,
          IPlaylistSongService
    {
        public PlaylistSongService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<PlaylistSong> AddInclude(
            IQueryable<PlaylistSong> query,
            PlaylistSongSearchObject? search = null)
        {
            return query
                .Include(x => x.Playlist)
                .Include(x => x.Song)
                    .ThenInclude(x => x.Artist);
        }

        protected override IQueryable<PlaylistSong> ApplyFilter(
            IQueryable<PlaylistSong> query,
            PlaylistSongSearchObject? search = null)
        {
            query = base.ApplyFilter(query, search);

            if (search == null)
                return query;

            if (search.PlaylistId.HasValue)
            {
                query = query.Where(x => x.PlaylistId == search.PlaylistId.Value);
            }

            if (search.SongId.HasValue)
            {
                query = query.Where(x => x.SongId == search.SongId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.ToLower();

                query = query.Where(x =>
                    x.Song != null &&
                    (
                        x.Song.Title.ToLower().Contains(fts) ||
                        (x.Song.Artist != null &&
                         x.Song.Artist.Name.ToLower().Contains(fts)) ||
                        (x.Playlist != null &&
                         x.Playlist.Name.ToLower().Contains(fts))
                    ));
            }

            return query;
        }

        protected override async Task BeforeInsert(
            PlaylistSong entity,
            PlaylistSongUpsertRequest request)
        {
            await ValidateRequest(request);

            entity.AddedAt = DateTime.UtcNow;

            await base.BeforeInsert(entity, request);
        }

        protected override async Task BeforeUpdate(
            PlaylistSong entity,
            PlaylistSongUpsertRequest request)
        {
            await ValidateRequest(request, entity.Id);

            await base.BeforeUpdate(entity, request);
        }

        private async Task ValidateRequest(
            PlaylistSongUpsertRequest request,
            int? playlistSongId = null)
        {
            if (request.PlaylistId <= 0)
                throw new UserException("PlaylistId je obavezan.");

            if (request.SongId <= 0)
                throw new UserException("SongId je obavezan.");

            var playlistExists = await _context.Set<Playlist>()
                .AnyAsync(x => x.Id == request.PlaylistId);

            if (!playlistExists)
                throw new UserException("Playlista nije pronađena.");

            var songExists = await _context.Set<Song>()
                .AnyAsync(x => x.Id == request.SongId);

            if (!songExists)
                throw new UserException("Pjesma nije pronađena.");

            var alreadyExists = await _context.Set<PlaylistSong>()
                .AnyAsync(x =>
                    x.PlaylistId == request.PlaylistId &&
                    x.SongId == request.SongId &&
                    (!playlistSongId.HasValue || x.Id != playlistSongId.Value));

            if (alreadyExists)
                throw new UserException("Pjesma je već dodana u playlistu.");
        }

        protected override PlaylistSongResponse MapToResponse(PlaylistSong entity)
        {
            return new PlaylistSongResponse
            {
                Id = entity.Id,
                PlaylistId = entity.PlaylistId,
                PlaylistName = entity.Playlist?.Name,
                SongId = entity.SongId,
                SongTitle = entity.Song?.Title,
                ArtistName = entity.Song?.Artist?.Name,
                CoverUrl = entity.Song?.CoverUrl,
                ExternalTrackId = entity.Song?.ExternalTrackId,
                DurationSeconds = entity.Song?.DurationSeconds,
                AddedAt = entity.AddedAt
            };
        }
    }
}
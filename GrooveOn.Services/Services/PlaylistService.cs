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
    public class PlaylistService
        : BaseCRUDService<PlaylistResponse, PlaylistSearchObject, Playlist, PlaylistUpsertRequest, PlaylistUpsertRequest>,
          IPlaylistService
    {
        public PlaylistService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<Playlist> AddInclude(IQueryable<Playlist> query, PlaylistSearchObject? search = null)
        {
            query = query
                .Include(x => x.User)
                .Include(x => x.PlaylistSongs);

            return query;
        }

        protected override IQueryable<Playlist> ApplyFilter(IQueryable<Playlist> query, PlaylistSearchObject? search = null)
        {
            query = base.ApplyFilter(query, search);

            if (search == null)
                return query;

            if (search.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == search.UserId.Value);
            }

            if (search.IsPublic.HasValue)
            {
                query = query.Where(x => x.IsPublic == search.IsPublic.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(fts) ||
                    (x.Description != null && x.Description.ToLower().Contains(fts)) ||
                    (x.User != null && x.User.Username.ToLower().Contains(fts)));
            }

            return query;
        }

        protected override async Task BeforeInsert(Playlist entity, PlaylistUpsertRequest request)
        {
            await ValidateRequest(request);

            entity.CreatedAt = DateTime.UtcNow;

            await base.BeforeInsert(entity, request);
        }

        protected override async Task BeforeUpdate(Playlist entity, PlaylistUpsertRequest request)
        {
            await ValidateRequest(request, entity.Id);

            await base.BeforeUpdate(entity, request);
        }

        private async Task ValidateRequest(PlaylistUpsertRequest request, int? playlistId = null)
        {
            if (request.UserId <= 0)
                throw new UserException("UserId je obavezan.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new UserException("Naziv playliste je obavezan.");

            var userExists = await _context.Set<User>()
                .AnyAsync(x => x.Id == request.UserId);

            if (!userExists)
                throw new UserException("Korisnik nije pronađen.");

            var playlistNameExists = await _context.Set<Playlist>()
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Name.ToLower() == request.Name.ToLower() &&
                    (!playlistId.HasValue || x.Id != playlistId.Value));

            if (playlistNameExists)
                throw new UserException("Već postoji playlista sa istim nazivom.");
        }

        protected override PlaylistResponse MapToResponse(Playlist entity)
        {
            return new PlaylistResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Username = entity.User?.Username,
                Name = entity.Name,
                Description = entity.Description,
                IsPublic = entity.IsPublic,
                CoverImageUrl = entity.CoverImageUrl,
                CreatedAt = entity.CreatedAt,
                SongCount = entity.PlaylistSongs?.Count ?? 0
            };
        }
    }
}
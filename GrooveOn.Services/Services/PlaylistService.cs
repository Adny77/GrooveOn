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

        protected override async Task BeforeInsert(Playlist entity, PlaylistUpsertRequest request)
        {
            await ValidateRequest(request);

            entity.CreatedAt = DateTime.UtcNow;

            var userId = request.UserId;

            var hasPremium = await _context.Subscriptions
                .Include(x => x.SubscriptionPlan)
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.IsActive &&
                    x.SubscriptionPlan != null &&
                    x.SubscriptionPlan.Name != "Basic Account" &&
                    (x.ExpiryDate == null || x.ExpiryDate > DateTime.Now)
                );

            if (!hasPremium)
            {
                var playlistCount = await _context.Playlists
                    .CountAsync(x => x.UserId == userId);

                if (playlistCount >= 3)
                {
                    throw new UserException(
                        "Basic users can create a maximum of 3 playlists. Upgrade to a premium subscription to create more playlists."
                    );
                }
            }

            await base.BeforeInsert(entity, request);
        }

        protected override IQueryable<Playlist> ApplyFilter(
    IQueryable<Playlist> query,
    PlaylistSearchObject? search = null)
        {
            query = base.ApplyFilter(query, search);

            if (search == null)
                return query;

            if (search.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == search.UserId.Value);
            }

            if (search.ExcludeUserId.HasValue)
            {
                query = query.Where(x => x.UserId != search.ExcludeUserId.Value);
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

        protected override async Task BeforeUpdate(Playlist entity, PlaylistUpsertRequest request)
        {
            await ValidateRequest(request, entity.Id);

            await base.BeforeUpdate(entity, request);
        }

        private async Task ValidateRequest(PlaylistUpsertRequest request, int? playlistId = null)
        {
            if (request.UserId <= 0)
                throw new UserException("UserId is required.");

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new UserException("Playlist name is required.");

            var userExists = await _context.Set<User>()
                .AnyAsync(x => x.Id == request.UserId);

            if (!userExists)
                throw new UserException("User was not found.");

            var playlistNameExists = await _context.Set<Playlist>()
                .AnyAsync(x =>
                    x.UserId == request.UserId &&
                    x.Name.ToLower() == request.Name.ToLower() &&
                    (!playlistId.HasValue || x.Id != playlistId.Value));

            if (playlistNameExists)
                throw new UserException("A playlist with the same name already exists.");
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
using System.Linq;
using System.Threading.Tasks;
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
        public PlayerService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<Player> ApplyFilter(IQueryable<Player> query,PlayerSearchObject search)
        {
            query = base.ApplyFilter(query, search);

            query = query
                .Include(x => x.User)
                .Include(x => x.Song);

            if (search.UserId.HasValue)
                query = query.Where(x => x.UserId == search.UserId.Value);

            if (search.SongId.HasValue)
                query = query.Where(x => x.SongId == search.SongId.Value);

            if (search.IsPlaying.HasValue)
                query = query.Where(x => x.IsPlaying == search.IsPlaying.Value);

            if (search.IsVisible.HasValue)
                query = query.Where(x => x.IsVisible == search.IsVisible.Value);

            return query;
        }

        protected override async Task BeforeInsert(Player entity,PlayerUpsertRequest request)
        {
            var existing = await _context.Set<Player>()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (existing != null)
                throw new UserException("Player state already exists for this user. Use update instead.");

            await base.BeforeInsert(entity, request);
        }

        protected override async Task BeforeUpdate(Player entity,PlayerUpsertRequest request)
        {
            entity.UpdatedAt = System.DateTime.UtcNow;
            await base.BeforeUpdate(entity, request);
        }

        protected override Player MapInsertToEntity(Player entity, PlayerUpsertRequest request)
        {
            entity.UpdatedAt = System.DateTime.UtcNow;
            return base.MapInsertToEntity(entity, request);
        }

        protected override PlayerResponse MapToResponse(Player entity)
        {
            return new PlayerResponse
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Username = entity.User?.Username,
                SongId = entity.SongId,
                SongTitle = entity.Song?.Title,
                SongCoverUrl = entity.Song?.CoverUrl,
                PreviewUrl = entity.Song?.PreviewUrl,
                CurrentSeconds = entity.CurrentSeconds,
                IsPlaying = entity.IsPlaying,
                IsVisible = entity.IsVisible,
                UpdatedAt = entity.UpdatedAt
            };
        }
    }
}
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class SubscriptionService
        : BaseCRUDService<SubscriptionResponse, SubscriptionSearchObject, Subscription, SubscriptionUpsertRequest, SubscriptionUpsertRequest>,
          ISubscriptionService
    {
        public SubscriptionService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<Subscription> AddInclude(
            IQueryable<Subscription> query,
            SubscriptionSearchObject search)
        {
            query = query
                .Include(x => x.User)
                .Include(x => x.SubscriptionPlan);

            return base.AddInclude(query, search);
        }

        protected override IQueryable<Subscription> ApplyFilter(
            IQueryable<Subscription> query,
            SubscriptionSearchObject search)
        {
            if (search.UserId.HasValue)
                query = query.Where(x => x.UserId == search.UserId.Value);

            if (search.SubscriptionPlanId.HasValue)
                query = query.Where(x => x.SubscriptionPlanId == search.SubscriptionPlanId.Value);

            if (search.IsActive.HasValue)
                query = query.Where(x => x.IsActive == search.IsActive.Value);

            if (search.OnlyExpired == true)
            {
                query = query.Where(x =>
                    x.ExpiryDate.HasValue &&
                    x.ExpiryDate.Value < DateTime.UtcNow);
            }

            if (search.OnlyValid == true)
            {
                query = query.Where(x =>
                    x.IsActive &&
                    (!x.ExpiryDate.HasValue || x.ExpiryDate.Value > DateTime.UtcNow));
            }

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.Trim().ToLower();

                query = query.Where(x =>
                    (x.User != null &&
                    (
                        x.User.Username.ToLower().Contains(fts) ||
                        x.User.FirstName.ToLower().Contains(fts) ||
                        x.User.LastName.ToLower().Contains(fts)
                    ))
                    ||
                    (x.SubscriptionPlan != null &&
                    x.SubscriptionPlan.Name.ToLower().Contains(fts)));
            }

            return base.ApplyFilter(query, search);
        }

        protected override SubscriptionResponse MapToResponse(Subscription entity)
        {
            var response = _mapper.Map<SubscriptionResponse>(entity);

            response.Username = entity.User?.Username;
            response.UserFullName = entity.User == null
                ? null
                : $"{entity.User.FirstName} {entity.User.LastName}";

            response.SubscriptionPlanName = entity.SubscriptionPlan?.Name;
            response.SubscriptionPlanCode = entity.SubscriptionPlan?.PlanCode;
            response.SubscriptionPlanPrice = entity.SubscriptionPlan?.Price ?? 0;
            response.SubscriptionPlanDurationDays = entity.SubscriptionPlan?.DurationDays ?? 0;

            return response;
        }

        protected override async Task BeforeInsert(Subscription entity, SubscriptionUpsertRequest request)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Id == request.UserId);
            if (!userExists)
                throw new NotFoundException("User was not found.");

            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(x => x.Id == request.SubscriptionPlanId);

            if (plan == null)
                throw new NotFoundException("Subscription plan was not found.");

            if (entity.StartDate == default)
                entity.StartDate = DateTime.UtcNow;

            if (plan.DurationDays == 0)
                entity.ExpiryDate = null;
            else
                entity.ExpiryDate ??= entity.StartDate.AddDays(plan.DurationDays);

            if (entity.IsActive)
            {
                var oldActiveSubscriptions = await _context.Subscriptions
                    .Where(x => x.UserId == entity.UserId && x.IsActive)
                    .ToListAsync();

                foreach (var oldSub in oldActiveSubscriptions)
                    oldSub.IsActive = false;
            }

            await base.BeforeInsert(entity, request);
        }

        protected override async Task BeforeUpdate(Subscription entity, SubscriptionUpsertRequest request)
        {
            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(x => x.Id == request.SubscriptionPlanId);

            if (plan == null)
                throw new NotFoundException("Subscription plan was not found.");

            if (plan.DurationDays == 0)
                entity.ExpiryDate = null;
            else if (!entity.ExpiryDate.HasValue)
                entity.ExpiryDate = entity.StartDate.AddDays(plan.DurationDays);

            if (entity.IsActive)
            {
                var oldActiveSubscriptions = await _context.Subscriptions
                    .Where(x =>
                        x.UserId == entity.UserId &&
                        x.IsActive &&
                        x.Id != entity.Id)
                    .ToListAsync();

                foreach (var oldSub in oldActiveSubscriptions)
                    oldSub.IsActive = false;
            }

            await base.BeforeUpdate(entity, request);
        }

        public async Task<SubscriptionResponse?> GetActiveByUserIdAsync(int userId)
        {
            var subscription = await _context.Subscriptions
                .Include(x => x.User)
                .Include(x => x.SubscriptionPlan)
                .Where(x =>
                    x.UserId == userId &&
                    x.IsActive &&
                    (!x.ExpiryDate.HasValue || x.ExpiryDate.Value > DateTime.UtcNow))
                .OrderByDescending(x => x.StartDate)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return null;

            return MapToResponse(subscription);
        }
    }
}

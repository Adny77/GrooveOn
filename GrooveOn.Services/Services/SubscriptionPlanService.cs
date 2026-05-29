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
    public class SubscriptionPlanService
        : BaseCRUDService<SubscriptionPlanResponse, SubscriptionPlanSearchObject, SubscriptionPlan, SubscriptionPlanUpsertRequest, SubscriptionPlanUpsertRequest>,
          ISubscriptionPlanService
    {
        public SubscriptionPlanService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<SubscriptionPlan> ApplyFilter(
            IQueryable<SubscriptionPlan> query,
            SubscriptionPlanSearchObject search)
        {
            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.Trim().ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(fts) ||
                    x.PlanCode.ToLower().Contains(fts) ||
                    (x.Description != null && x.Description.ToLower().Contains(fts)));
            }

            if (!string.IsNullOrWhiteSpace(search.Name))
            {
                var name = search.Name.Trim().ToLower();
                query = query.Where(x => x.Name.ToLower().Contains(name));
            }

            if (search.IsActive.HasValue)
                query = query.Where(x => x.IsActive == search.IsActive.Value);

            return base.ApplyFilter(query, search);
        }

        protected override async Task BeforeDelete(SubscriptionPlan entity)
        {
            var hasSubscriptions = await _context.Subscriptions
                .AnyAsync(x => x.SubscriptionPlanId == entity.Id);

            if (hasSubscriptions)
                throw new UserException(
                    $"Subscription plan '{entity.Name}' cannot be deleted because it has existing subscriptions. " +
                    "Deactivate the plan instead.");

            await base.BeforeDelete(entity);
        }

        protected override async Task BeforeInsert(SubscriptionPlan entity, SubscriptionPlanUpsertRequest request)
        {
            NormalizePlanCode(entity);
            await ValidatePlanCodeAsync(entity.PlanCode);

            await base.BeforeInsert(entity, request);
        }

        protected override async Task BeforeUpdate(SubscriptionPlan entity, SubscriptionPlanUpsertRequest request)
        {
            var planCode = request.PlanCode.Trim().ToLowerInvariant();

            var exists = await _context.SubscriptionPlans
                .AnyAsync(x => x.Id != entity.Id && x.PlanCode == planCode);

            if (exists)
                throw new UserException("Subscription plan code is already in use.");

            await base.BeforeUpdate(entity, request);
        }

        protected override SubscriptionPlan MapInsertToEntity(
            SubscriptionPlan entity,
            SubscriptionPlanUpsertRequest request)
        {
            var mapped = base.MapInsertToEntity(entity, request);
            NormalizePlanCode(mapped);
            return mapped;
        }

        protected override void MapUpdateToEntity(
            SubscriptionPlan entity,
            SubscriptionPlanUpsertRequest request)
        {
            base.MapUpdateToEntity(entity, request);
            NormalizePlanCode(entity);
        }

        private static void NormalizePlanCode(SubscriptionPlan entity)
        {
            entity.PlanCode = entity.PlanCode.Trim().ToLowerInvariant();
        }

        private async Task ValidatePlanCodeAsync(string planCode)
        {
            if (string.IsNullOrWhiteSpace(planCode))
                throw new UserException("Subscription plan code is required.");

            var exists = await _context.SubscriptionPlans.AnyAsync(x => x.PlanCode == planCode);
            if (exists)
                throw new UserException("Subscription plan code is already in use.");
        }
    }
}

using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;

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
    }
}
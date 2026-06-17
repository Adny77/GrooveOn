using GrooveOn.WebAPI.Controllers;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionPlanController
        : BaseCRUDController<SubscriptionPlanResponse, SubscriptionPlanSearchObject, SubscriptionPlanUpsertRequest, SubscriptionPlanUpsertRequest>
    {
        public SubscriptionPlanController(ISubscriptionPlanService service)
            : base(service)
        {
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("")]
        public override Task<PagedResult<SubscriptionPlanResponse>> Get([FromQuery] SubscriptionPlanSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("{id}")]
        public override Task<SubscriptionPlanResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public override Task<SubscriptionPlanResponse> Create([FromBody] SubscriptionPlanUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public override Task<SubscriptionPlanResponse?> Update(int id, [FromBody] SubscriptionPlanUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}

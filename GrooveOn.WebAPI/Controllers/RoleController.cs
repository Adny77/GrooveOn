using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class RoleController : BaseCRUDController<RoleResponse, BaseSearchObject, RoleUpsertRequest, RoleUpsertRequest>
    {
        public RoleController(IRoleService service) : base(service)
        {
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("")]
        public override Task<PagedResult<RoleResponse>> Get([FromQuery] BaseSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}")]
        public override Task<RoleResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public override Task<RoleResponse> Create([FromBody] RoleUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public override Task<RoleResponse?> Update(int id, [FromBody] RoleUpsertRequest request)
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

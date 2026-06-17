using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.WebAPI.Controllers
{
    public class AnswerController
        : BaseCRUDController<AnswerResponse, AnswerSearchObject, AnswerUpsertRequest, AnswerUpsertRequest>
    {
        public AnswerController(IAnswerService service) : base(service)
        {
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("")]
        public override Task<PagedResult<AnswerResponse>> Get([FromQuery] AnswerSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("{id}")]
        public override Task<AnswerResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public override Task<AnswerResponse> Create([FromBody] AnswerUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public override Task<AnswerResponse?> Update(int id, [FromBody] AnswerUpsertRequest request)
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

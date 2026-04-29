using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Models.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class QuestionController 
        : BaseCRUDController<QuestionResponse, QuestionSearchObject, QuestionUpsertRequest, QuestionUpsertRequest>
    {
        public QuestionController(IQuestionService service) : base(service)
        {
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("")]
        public override Task<PagedResult<QuestionResponse>> Get([FromQuery] QuestionSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public override Task<QuestionResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public override Task<QuestionResponse> Create([FromBody] QuestionUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpPut("{id}")]
        public override Task<QuestionResponse?> Update(int id, [FromBody] QuestionUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}

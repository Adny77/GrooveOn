using System.Security.Claims;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Models.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.WebAPI.Controllers
{
    public class QuestionController 
        : BaseCRUDController<QuestionResponse, QuestionSearchObject, QuestionUpsertRequest, QuestionUpsertRequest>
    {
        public QuestionController(IQuestionService service) : base(service)
        {
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("")]
        public override Task<PagedResult<QuestionResponse>> Get([FromQuery] QuestionSearchObject? search = null)
        {
            search ??= new QuestionSearchObject();

            if (!User.IsInRole(Roles.Admin))
                search.UserId = GetCurrentUserId();

            return base.Get(search);
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("{id}")]
        public override async Task<QuestionResponse?> GetById(int id)
        {
            var question = await base.GetById(id);
            if (question == null)
                return null;

            if (User.IsInRole(Roles.Admin) || question.UserId == GetCurrentUserId())
                return question;

            return null;
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost]
        public override Task<QuestionResponse> Create([FromBody] QuestionUpsertRequest request)
        {
            request.UserId = GetCurrentUserId();
            return base.Create(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public override Task<QuestionResponse?> Update(int id, [FromBody] QuestionUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid token.");

            return userId;
        }
    }
}

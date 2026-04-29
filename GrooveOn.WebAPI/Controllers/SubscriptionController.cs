using GrooveOn.API.Controllers;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrooveOn.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController
        : BaseCRUDController<SubscriptionResponse, SubscriptionSearchObject, SubscriptionUpsertRequest, SubscriptionUpsertRequest>
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
            : base(subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public override Task<PagedResult<SubscriptionResponse>> Get([FromQuery] SubscriptionSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public override Task<SubscriptionResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public override Task<SubscriptionResponse> Create([FromBody] SubscriptionUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public override Task<SubscriptionResponse?> Update(int id, [FromBody] SubscriptionUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("active/{userId}")]
        public async Task<ActionResult<SubscriptionResponse?>> GetActiveByUserId(int userId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var loggedInUserId))
                return Unauthorized("Invalid token.");

            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && userId != loggedInUserId)
                return Forbid();

            var result = await _subscriptionService.GetActiveByUserIdAsync(userId);

            return Ok(result);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("my-active")]
        public async Task<ActionResult<SubscriptionResponse?>> GetMyActive()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var loggedInUserId))
                return Unauthorized("Invalid token.");

            var result = await _subscriptionService.GetActiveByUserIdAsync(loggedInUserId);

            return Ok(result);
        }
    }
}

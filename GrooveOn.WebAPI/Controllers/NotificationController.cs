using System.Security.Claims;
using GrooveOn.API.Controllers;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Models.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController
        : BaseCRUDController<NotificationResponse, NotificationSearchObject, NotificationUpsertRequest, NotificationUpsertRequest>
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService service)
            : base(service)
        {
            _notificationService = service;
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("")]
        public override Task<PagedResult<NotificationResponse>> Get(
            [FromQuery] NotificationSearchObject? search = null)
        {
            search ??= new NotificationSearchObject();
            search.UserId = GetCurrentUserId();
            return base.Get(search);
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("{id}")]
        public override async Task<NotificationResponse?> GetById(int id)
        {
            var result = await base.GetById(id);
            return result?.UserId == GetCurrentUserId() ? result : null;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public override Task<NotificationResponse> Create([FromBody] NotificationUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPut("{id}/mark-read")]
        public async Task<ActionResult<NotificationResponse>> MarkAsRead(int id)
        {
            var result = await _notificationService.MarkAsReadAsync(id, GetCurrentUserId());
            return result == null ? NotFound() : Ok(result);
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

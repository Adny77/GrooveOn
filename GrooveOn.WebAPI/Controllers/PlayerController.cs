using System.Security.Claims;
using System.Threading.Tasks;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.WebAPI.Controllers
{
    public class PlayerController
        : BaseCRUDController<PlayerResponse, PlayerSearchObject, PlayerUpsertRequest, PlayerUpsertRequest>
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService service)
            : base(service)
        {
            _playerService = service;
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("")]
        public override Task<PagedResult<PlayerResponse>> Get([FromQuery] PlayerSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("{id}")]
        public override Task<PlayerResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost]
        public override Task<PlayerResponse> Create([FromBody] PlayerUpsertRequest request)
        {
            request.UserId = GetCurrentUserId();
            return base.Create(request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPut("{id}")]
        public override Task<PlayerResponse?> Update(int id, [FromBody] PlayerUpsertRequest request)
        {
            request.UserId = GetCurrentUserId();
            return base.Update(id, request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost("random/play")]
        public async Task<PlayerResponse> PlayRandomMusic([FromBody] PlayerUpsertRequest request)
        {
            request.UserId = GetCurrentUserId();
            return await _playerService.PlayRandomMusicAsync(request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost("random/next")]
        public async Task<PlayerResponse> PlayNext([FromBody] PlayerUpsertRequest request)
        {
            request.UserId = GetCurrentUserId();
            return await _playerService.PlayNextRandomMusicAsync(request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost("random/previous")]
        public async Task<PlayerResponse> PlayPrevious([FromBody] PlayerUpsertRequest request)
        {
            request.UserId = GetCurrentUserId();
            return await _playerService.PlayPreviousRandomMusicAsync(request);
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

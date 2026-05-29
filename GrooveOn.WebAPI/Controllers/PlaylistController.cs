using System.Security.Claims;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController
        : BaseCRUDController<PlaylistResponse, PlaylistSearchObject, PlaylistUpsertRequest, PlaylistUpsertRequest>
    {
        public PlaylistController(IPlaylistService service)
            : base(service)
        {
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("")]
        public override Task<PagedResult<PlaylistResponse>> Get([FromQuery] PlaylistSearchObject? search = null)
        {
            search ??= new PlaylistSearchObject();

            if (search.IsPublic != true)
                search.UserId = GetCurrentUserId();

            return base.Get(search);
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("{id}")]
        public override async Task<PlaylistResponse?> GetById(int id)
        {
            var playlist = await base.GetById(id);
            if (playlist == null)
                return null;

            if (playlist.UserId == GetCurrentUserId() || playlist.IsPublic)
                return playlist;

            return null;
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost]
        public override Task<PlaylistResponse> Create([FromBody] PlaylistUpsertRequest request)
        {
            request.UserId = GetCurrentUserId();
            return base.Create(request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPut("{id}")]
        public override async Task<PlaylistResponse?> Update(int id, [FromBody] PlaylistUpsertRequest request)
        {
            var playlist = await base.GetById(id);
            if (playlist == null)
                return null;

            if (playlist.UserId != GetCurrentUserId())
                throw new ForbiddenException("You can only update your own playlists.");

            request.UserId = GetCurrentUserId();
            return await base.Update(id, request);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(claim, out var userId))
                throw new UnauthorizedAccessException("Invalid token.");
            return userId;
        }

        [Authorize(Roles = Roles.User)]
        [HttpDelete("{id}")]
        public override async Task<bool> Delete(int id)
        {
            var playlist = await base.GetById(id);
            if (playlist == null)
                return false;

            if (playlist.UserId != GetCurrentUserId())
                throw new ForbiddenException("You can only delete your own playlists.");

            return await base.Delete(id);
        }
    }
}

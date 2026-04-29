using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
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

        [Authorize(Roles = "User")]
        [HttpGet("")]
        public override Task<PagedResult<PlaylistResponse>> Get([FromQuery] PlaylistSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public override Task<PlaylistResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public override Task<PlaylistResponse> Create([FromBody] PlaylistUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public override Task<PlaylistResponse?> Update(int id, [FromBody] PlaylistUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}

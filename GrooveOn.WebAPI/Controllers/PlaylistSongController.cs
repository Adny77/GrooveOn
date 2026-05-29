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
    public class PlaylistSongController
        : BaseCRUDController<PlaylistSongResponse, PlaylistSongSearchObject, PlaylistSongUpsertRequest, PlaylistSongUpsertRequest>
    {
        public PlaylistSongController(IPlaylistSongService service)
            : base(service)
        {
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("")]
        public override Task<PagedResult<PlaylistSongResponse>> Get([FromQuery] PlaylistSongSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("{id}")]
        public override Task<PlaylistSongResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPost]
        public override Task<PlaylistSongResponse> Create([FromBody] PlaylistSongUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpPut("{id}")]
        public override Task<PlaylistSongResponse?> Update(int id, [FromBody] PlaylistSongUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = Roles.User)]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }
    }
}

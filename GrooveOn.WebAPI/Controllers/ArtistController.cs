using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistController : BaseCRUDController<
        ArtistResponse,
        ArtistSearchObject,
        ArtistUpsertRequest,
        ArtistUpsertRequest>
    {
        public ArtistController(
            IArtistService service
        ) : base(service)
        {
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("")]
        public override Task<PagedResult<ArtistResponse>> Get([FromQuery] ArtistSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("{id}")]
        public override Task<ArtistResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public override Task<ArtistResponse> Create([FromBody] ArtistUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public override Task<ArtistResponse?> Update(int id, [FromBody] ArtistUpsertRequest request)
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

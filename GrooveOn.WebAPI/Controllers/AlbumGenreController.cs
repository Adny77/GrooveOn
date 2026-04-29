using GrooveOn.Model.Requests;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class AlbumGenreController
        : BaseCRUDController<AlbumGenreResponse, AlbumGenreSearchObject, AlbumGenreUpsertRequest, AlbumGenreUpsertRequest>
    {
        public AlbumGenreController(IAlbumGenreService service) : base(service)
        {
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public override Task<PagedResult<AlbumGenreResponse>> Get([FromQuery] AlbumGenreSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public override Task<AlbumGenreResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public override Task<AlbumGenreResponse> Create([FromBody] AlbumGenreUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public override Task<AlbumGenreResponse?> Update(int id, [FromBody] AlbumGenreUpsertRequest request)
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

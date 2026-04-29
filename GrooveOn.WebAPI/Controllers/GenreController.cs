using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class GenreController : BaseCRUDController<GenreResponse, GenreSearchObject, GenreUpsertRequest, GenreUpsertRequest>
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService service) : base(service)
        {
            _genreService = service;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("")]
        public override Task<PagedResult<GenreResponse>> Get([FromQuery] GenreSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public override Task<GenreResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public override Task<GenreResponse> Create([FromBody] GenreUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public override Task<GenreResponse?> Update(int id, [FromBody] GenreUpsertRequest request)
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

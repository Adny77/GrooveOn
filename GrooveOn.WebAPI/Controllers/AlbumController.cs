using GrooveOn.Model.Requests;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class AlbumController
        : BaseCRUDController<AlbumResponse, AlbumSearchObject, AlbumUpsertRequest, AlbumUpsertRequest>
    {
        private readonly IAlbumService _albumService;

        public AlbumController(IAlbumService service) : base(service)
        {
            _albumService = service;
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("")]
        public override Task<PagedResult<AlbumResponse>> Get([FromQuery] AlbumSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "User,Admin")]
        [HttpGet("{id}")]
        public override Task<AlbumResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public override Task<AlbumResponse> Create([FromBody] AlbumUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public override Task<AlbumResponse?> Update(int id, [FromBody] AlbumUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("preview-deezer")]
        public async Task<AlbumPreviewResponse> PreviewDeezerAlbum(
            [FromBody] AlbumUpsertRequest request)
        {
            return await _albumService.PreviewDeezerAlbumAsync(request);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("save-deezer")]
        public async Task<AlbumSaveResponse> SaveDeezerAlbum(
            [FromBody] AlbumUpsertRequest request)
        {
            return await _albumService.SaveDeezerAlbumAsync(request);
        }
    }
}

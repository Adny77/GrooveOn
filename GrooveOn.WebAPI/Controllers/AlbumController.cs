using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
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

        [HttpPost("preview-deezer")]
        public async Task<AlbumPreviewResponse> PreviewDeezerAlbum(
            [FromBody] AlbumUpsertRequest request)
        {
            return await _albumService.PreviewDeezerAlbumAsync(request);
        }

        [HttpPost("save-deezer")]
        public async Task<AlbumSaveResponse> SaveDeezerAlbum(
            [FromBody] AlbumUpsertRequest request)
        {
            return await _albumService.SaveDeezerAlbumAsync(request);
        }
    }
}
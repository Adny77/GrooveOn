using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class SongController 
        : BaseCRUDController<SongResponse, SongSearchObject, SongUpsertRequest, SongUpsertRequest>
    {
        private readonly ISongService _songService;

        public SongController(ISongService service) : base(service)
        {
            _songService = service;
        }

        [HttpPost("check-duplicates")]
        public async Task<SongDuplicateCheckResponse> CheckDuplicates(
            [FromBody] SongDuplicateCheckRequest request)
        {
            return await _songService.CheckDuplicatesAsync(request);
        }

        [HttpPost("bulk-save-deezer")]
        public async Task<SongBulkInsertResponse> BulkSaveDeezerSongs(
            [FromBody] SongBulkInsertRequest request)
        {
            return await _songService.BulkInsertDeezerSongsAsync(request);
        }
    }
}
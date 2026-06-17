using GrooveOn.Model.Requests;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.WebAPI.Controllers
{
    public class SongController
        : BaseCRUDController<SongResponse, SongSearchObject, SongUpsertRequest, SongUpsertRequest>
    {
        private readonly ISongService _songService;

        public SongController(ISongService service) : base(service)
        {
            _songService = service;
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("")]
        public override Task<PagedResult<SongResponse>> Get([FromQuery] SongSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("{id}")]
        public override Task<SongResponse?> GetById(int id)
        {
            return base.GetById(id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public override Task<SongResponse> Create([FromBody] SongUpsertRequest request)
        {
            return base.Create(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public override Task<SongResponse?> Update(int id, [FromBody] SongUpsertRequest request)
        {
            return base.Update(id, request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public override Task<bool> Delete(int id)
        {
            return base.Delete(id);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("check-duplicates")]
        public async Task<SongDuplicateCheckResponse> CheckDuplicates(
            [FromBody] SongDuplicateCheckRequest request)
        {
            return await _songService.CheckDuplicatesAsync(request);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost("bulk-save-deezer")]
        public async Task<SongBulkInsertResponse> BulkSaveDeezerSongs(
            [FromBody] SongBulkInsertRequest request)
        {
            return await _songService.BulkInsertDeezerSongsAsync(request);
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("recommended")]
        public async Task<ActionResult<List<SongResponse>>> GetRecommended([FromQuery] int take = 4)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("Invalid token.");

            var result = await _songService.GetRecommendedForUserAsync(userId, take);

            return Ok(result);
        }
    }
}

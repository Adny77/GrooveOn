using System.Threading.Tasks;
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class PlayerController
        : BaseCRUDController<PlayerResponse, PlayerSearchObject, PlayerUpsertRequest, PlayerUpsertRequest>
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService service)
            : base(service)
        {
            _playerService = service;
        }


        [HttpPost("random/play")]
        public async Task<PlayerResponse> PlayRandomMusic([FromBody] PlayerUpsertRequest request)
        {
            return await _playerService.PlayRandomMusicAsync(request);
        }


        [HttpPost("random/next")]
        public async Task<PlayerResponse> PlayNext([FromBody] PlayerUpsertRequest request)
        {
            return await _playerService.PlayNextRandomMusicAsync(request);
        }

        [HttpPost("random/previous")]
        public async Task<PlayerResponse> PlayPrevious([FromBody] PlayerUpsertRequest request)
        {
            return await _playerService.PlayPreviousRandomMusicAsync(request);
        }
    }
}
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    public class MusicSearchEngineController 
        : BaseController<MusicSearchItemResponse, MusicSearchSearchObject>
    {
        public MusicSearchEngineController(
            IMusicSearchEngineService service
        ) : base(service)
        {
        }

        [Authorize(Roles = "User")]
        [HttpGet("")]
        public override Task<PagedResult<MusicSearchItemResponse>> Get([FromQuery] MusicSearchSearchObject? search = null)
        {
            return base.Get(search);
        }

        [Authorize(Roles = "User")]
        [HttpGet("{id}")]
        public override Task<MusicSearchItemResponse?> GetById(int id)
        {
            return base.GetById(id);
        }
    }
}

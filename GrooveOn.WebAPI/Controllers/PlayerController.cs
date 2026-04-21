using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : BaseCRUDController<
        PlayerResponse,
        PlayerSearchObject,
        PlayerUpsertRequest,
        PlayerUpsertRequest>
    {
        public PlayerController(IPlayerService service) : base(service)
        {
        }
    }
}
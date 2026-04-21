using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
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
    }
}
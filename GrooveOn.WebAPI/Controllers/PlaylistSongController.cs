using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistSongController
        : BaseCRUDController<PlaylistSongResponse, PlaylistSongSearchObject, PlaylistSongUpsertRequest, PlaylistSongUpsertRequest>
    {
        public PlaylistSongController(IPlaylistSongService service)
            : base(service)
        {
        }
    }
}
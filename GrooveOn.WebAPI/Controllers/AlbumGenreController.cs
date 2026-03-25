using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;

namespace GrooveOn.API.Controllers
{
    public class AlbumGenreController
        : BaseCRUDController<AlbumGenreResponse, AlbumGenreSearchObject, AlbumGenreUpsertRequest, AlbumGenreUpsertRequest>
    {
        public AlbumGenreController(IAlbumGenreService service) : base(service)
        {
        }
    }
}
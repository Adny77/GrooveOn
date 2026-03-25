using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;

namespace GrooveOn.API.Controllers
{
    public class GenreController : BaseCRUDController<GenreResponse, GenreSearchObject, GenreUpsertRequest, GenreUpsertRequest>
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService service) : base(service)
        {
            _genreService = service;
        }
    }
}
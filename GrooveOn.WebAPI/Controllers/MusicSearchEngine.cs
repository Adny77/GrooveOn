using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;

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
    }
}
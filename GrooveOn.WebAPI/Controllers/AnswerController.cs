using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;

namespace GrooveOn.API.Controllers
{
    public class AnswerController
        : BaseCRUDController<AnswerResponse, AnswerSearchObject, AnswerUpsertRequest, AnswerUpsertRequest>
    {
        public AnswerController(IAnswerService service) : base(service)
        {
        }
    }
}
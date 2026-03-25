using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Interfaces;

namespace GrooveOn.API.Controllers
{
    public class QuestionController 
        : BaseCRUDController<QuestionResponse, BaseSearchObject, QuestionUpsertRequest, QuestionUpsertRequest>
    {
        public QuestionController(IQuestionService service) : base(service)
        {
        }
    }
}
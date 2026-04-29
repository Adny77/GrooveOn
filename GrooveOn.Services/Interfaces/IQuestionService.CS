using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Models.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IQuestionService 
        : ICRUDService<QuestionResponse, QuestionSearchObject, QuestionUpsertRequest, QuestionUpsertRequest>
    {
    }
}
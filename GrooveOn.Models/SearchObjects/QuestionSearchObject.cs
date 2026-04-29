using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Models.SearchObjects
{
    public class QuestionSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }

        public string? Status { get; set; }
    }
}
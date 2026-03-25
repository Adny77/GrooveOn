using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Model.RequestObjects
{
    public class AnswerUpsertRequest
    {
        [Required]
        public int QuestionId { get; set; }

        [Required]
        public int AdminId { get; set; }

        [Required]
        [MinLength(2)]
        public string Message { get; set; } = string.Empty;
    }
}
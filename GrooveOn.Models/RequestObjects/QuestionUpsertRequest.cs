using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Model.RequestObjects
{
    public class QuestionUpsertRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MinLength(3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MinLength(5)]
        public string Content { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string? Answer { get; set; }
    }
}
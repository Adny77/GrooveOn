using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Answer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        [ForeignKey(nameof(AdminId))]
        public int AdminId { get; set; }
        public User? Admin { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
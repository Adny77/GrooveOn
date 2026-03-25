using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class QuestionResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Answer { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? AnsweredAt { get; set; }
    }
}
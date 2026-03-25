using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class AnswerResponse
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }
        public string? QuestionTitle { get; set; }

        public int AdminId { get; set; }
        public string? AdminUserName { get; set; }

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
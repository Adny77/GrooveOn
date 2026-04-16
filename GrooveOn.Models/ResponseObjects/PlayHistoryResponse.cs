using System;

namespace GrooveOn.Model.ResponseObject
{
    public class PlayHistoryResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }

        public int SongId { get; set; }
        public string? SongTitle { get; set; }

        public DateTime PlayedAt { get; set; }
    }
}
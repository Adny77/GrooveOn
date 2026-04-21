using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class PlayerResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string? Username { get; set; }

        public int SongId { get; set; }
        public string? SongTitle { get; set; }
        public string? SongCoverUrl { get; set; }
        public string? PreviewUrl { get; set; }

        public int CurrentSeconds { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsVisible { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
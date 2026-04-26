using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class PlaylistResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }

        public string? CoverImageUrl { get; set; }

        public DateTime CreatedAt { get; set; }

        public int SongCount { get; set; }
    }
}
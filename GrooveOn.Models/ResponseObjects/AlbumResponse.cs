using System;

namespace GrooveOn.Model.Responses
{
    public class AlbumResponse
    {
        public int Id { get; set; }

        public string? ExternalAlbumId { get; set; }

        public string Source { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int ArtistId { get; set; }

        public string ArtistName { get; set; } = string.Empty;

        public DateTime? ReleaseDate { get; set; }

        public string? CoverUrl { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public int SongCount { get; set; }
    }
}
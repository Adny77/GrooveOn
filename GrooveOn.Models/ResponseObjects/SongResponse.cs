using System;

namespace GrooveOn.Model.Responses
{
    public class SongResponse
    {
        public int Id { get; set; }
        public string? ExternalTrackId { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public int ArtistId { get; set; }
        public string ArtistName { get; set; } = string.Empty;

        public int? AlbumId { get; set; }
        public string? AlbumTitle { get; set; }

        public int DurationSeconds { get; set; }
        public string? PreviewUrl { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public bool IsActive { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? WhyRecommended { get; set; }
    }
}
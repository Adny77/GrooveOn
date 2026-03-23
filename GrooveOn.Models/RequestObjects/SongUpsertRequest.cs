using System;

namespace GrooveOn.Model.Requests
{
    public class SongUpsertRequest
    {
        public string ExternalTrackId { get; set; } = string.Empty;
        public string Source { get; set; } = "Deezer";
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string? AlbumTitle { get; set; }
        public int DurationSeconds { get; set; }
        public string? PreviewUrl { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}
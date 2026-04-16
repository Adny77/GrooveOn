using System;

namespace GrooveOn.Model.RequestObjects
{
    public class ResolveAlbumRequest
    {
        public string? ExternalAlbumId { get; set; }
        public string? Title { get; set; }
        public string? Source { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string? Description { get; set; }

        public bool AllowNull { get; set; } = false;
        public bool IncludeDetails { get; set; } = false;
    }
}
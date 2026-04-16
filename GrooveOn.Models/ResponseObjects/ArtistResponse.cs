using System;

namespace GrooveOn.Model.ResponseObject
{
    public class ArtistResponse
    {
        public int Id { get; set; }

        public string? ExternalArtistId { get; set; }

        public string Source { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Picture { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
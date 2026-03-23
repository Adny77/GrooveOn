using System;
using System.Collections.Generic;

namespace GrooveOn.Model.Requests
{
    public class AlbumUpsertRequest
    {
        public string ExternalAlbumId { get; set; } = string.Empty;
        public string ExternalArtistId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;

        public string? CoverUrl { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public List<SongUpsertRequest> Tracks { get; set; }
    }
}
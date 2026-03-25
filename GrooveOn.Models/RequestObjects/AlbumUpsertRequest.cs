using System;
using System.Collections.Generic;
using GrooveOn.Model.RequestObjects;

namespace GrooveOn.Model.Requests
{
    public class AlbumUpsertRequest
    {
        public string ExternalAlbumId { get; set; } = string.Empty;
        public string? ExternalArtistId { get; set; }

        public string Source { get; set; } = "Deezer";

        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;

        public string? CoverUrl { get; set; }
        public string? Description { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public List<GenreUpsertRequest> Genres { get; set; }

        public List<SongUpsertRequest> Tracks { get; set; }
    }
}
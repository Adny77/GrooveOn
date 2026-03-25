using System;

namespace GrooveOn.Model.Responses
{
    public class AlbumGenreResponse
    {
        public int Id { get; set; }

        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; } = string.Empty;

        public int GenreId { get; set; }
        public string GenreName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
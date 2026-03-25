using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class GenreResponse
    {
        public int Id { get; set; }
        public string ExternalGenreId { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
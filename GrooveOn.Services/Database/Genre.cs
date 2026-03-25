using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Services.Database
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        public string? ExternalGenreId { get; set; }

        public string Source { get; set; } = "Deezer";

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();        
    }
}
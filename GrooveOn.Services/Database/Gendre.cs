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

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
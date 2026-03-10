using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Services.Database
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        public string? ExternalArtistId { get; set; }

        public string Source { get; set; } = "Deezer";

        public string Name { get; set; } = string.Empty;

        public string? Biography { get; set; }

        public string? Country { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Album> Albums { get; set; } = new List<Album>();

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        public string? ExternalAlbumId { get; set; }

        public string Source { get; set; } = "Deezer";

        public string Title { get; set; } = string.Empty;

        [ForeignKey(nameof(ArtistId))]
        public int ArtistId { get; set; }

        public Artist? Artist { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public string? CoverUrl { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}
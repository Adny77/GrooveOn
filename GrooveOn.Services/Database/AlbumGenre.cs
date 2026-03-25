using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class AlbumGenre
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(AlbumId))]
        public int AlbumId { get; set; }
        public Album? Album {get; set;}

        [ForeignKey(nameof(GenreId))]
        public int GenreId { get; set; }
        public Genre? Genre {get; set;}

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<AlbumGenre> AlbumGenres { get; set; } = new List<AlbumGenre>();

    }
}
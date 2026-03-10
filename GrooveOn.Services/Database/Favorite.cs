using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Favorite
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }

        public User? User { get; set; }

        [ForeignKey(nameof(SongId))]
        public int SongId { get; set; }

        public Song? Song { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
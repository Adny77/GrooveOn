using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class PlayHistory
    {
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [ForeignKey(nameof(SongId))]
        public int SongId { get; set; }
        public Song Song { get; set; } = null!;

        public DateTime PlayedAt { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class PlaylistSong
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(PlaylistId))]
        public int PlaylistId { get; set; }

        public Playlist? Playlist { get; set; }

        [ForeignKey(nameof(SongId))]
        public int SongId { get; set; }

        public Song? Song { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Song
    {
        [Key]
        public int Id { get; set; }

        public string? ExternalTrackId { get; set; }

        public string Source { get; set; } = "Deezer";

        public string Title { get; set; } = string.Empty;

        [ForeignKey(nameof(ArtistId))]
        public int ArtistId { get; set; }

        public Artist? Artist { get; set; }

        [ForeignKey(nameof(AlbumId))]
        public int? AlbumId { get; set; }

        public Album? Album { get; set; }

        public int? DurationSeconds { get; set; }

        public string? PreviewUrl { get; set; }

        public string? CoverUrl { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastSyncedAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PlaylistSong> PlaylistSongs { get; set; } = new List<PlaylistSong>();

        public ICollection<PlayHistory> ListeningHistories { get; set; } = new List<PlayHistory>();
    }
}
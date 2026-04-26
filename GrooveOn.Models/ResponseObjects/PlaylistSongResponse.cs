using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class PlaylistSongResponse
    {
        public int Id { get; set; }

        public int PlaylistId { get; set; }

        public string? PlaylistName { get; set; }

        public int SongId { get; set; }

        public string? SongTitle { get; set; }

        public string? ArtistName { get; set; }

        public string? CoverUrl { get; set; }

        public string? ExternalTrackId { get; set; }

        public int? DurationSeconds { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
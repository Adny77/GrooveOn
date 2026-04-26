namespace GrooveOn.Model.SearchObjects
{
    public class PlaylistSongSearchObject : BaseSearchObject
    {
        public int? PlaylistId { get; set; }

        public int? SongId { get; set; }

        public string? FTS { get; set; }
    }
}
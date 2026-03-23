namespace GrooveOn.Model.SearchObjects
{
    public class SongSearchObject : BaseSearchObject
    {
        public int? ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public bool? IsActive { get; set; }
    }
}
namespace GrooveOn.Model.SearchObjects
{
    public class SongSearchObject : BaseSearchObject
    {
        public int? ArtistId { get; set; }
        public int? AlbumId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IncludeArtist {get; set;}
        public bool? IncludeAlbum {get; set;}

    }
}
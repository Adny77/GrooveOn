namespace GrooveOn.Model.SearchObjects
{
    public class AlbumGenreSearchObject : BaseSearchObject
    {
        public int? AlbumId { get; set; }
        public int? GenreId { get; set; }
        public string? FTS { get; set; }
    }
}
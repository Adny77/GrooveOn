namespace GrooveOn.Model.SearchObjects
{
    public class ArtistSearchObject : BaseSearchObject
    {
        public string? Name { get; set; }

        public string? ExternalArtistId { get; set; }

        public string? Source { get; set; }

        public bool IncludeAlbums { get; set; }
        public bool IncludeSongs { get; set; }
    }
}
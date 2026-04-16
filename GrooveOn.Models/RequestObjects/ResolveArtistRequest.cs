namespace GrooveOn.Model.RequestObjects
{
    public class ResolveArtistRequest
    {
        public string? ExternalArtistId { get; set; }
        public string? ArtistName { get; set; }
        public string? Source { get; set; }
        public string? ArtistPicture { get; set; }
    }
}
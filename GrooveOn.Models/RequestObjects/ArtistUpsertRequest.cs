namespace GrooveOn.Model.RequestObjects
{
    public class ArtistUpsertRequest
    {
        public string? ExternalArtistId { get; set; }

        public string? Source { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Picture { get; set; }
    }
}
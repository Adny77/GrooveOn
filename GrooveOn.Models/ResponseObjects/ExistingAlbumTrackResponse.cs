namespace GrooveOn.Model.Responses
{
    public class ExistingAlbumTrackResponse
    {
        public string ExternalTrackId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool AlreadyExists { get; set; }
    }
}
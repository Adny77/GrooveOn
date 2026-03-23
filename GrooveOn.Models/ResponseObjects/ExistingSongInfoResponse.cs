namespace GrooveOn.Model.Responses
{
    public class ExistingSongInfoResponse
    {
        public int Id { get; set; }
        public string? ExternalTrackId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string? AlbumTitle { get; set; }
        public string? CoverUrl { get; set; }
    }
}
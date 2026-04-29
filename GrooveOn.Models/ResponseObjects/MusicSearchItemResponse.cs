public class MusicSearchItemResponse
{
    public string Type { get; set; } = string.Empty; 

    public int Id { get; set; }
    public string? ExternalTrackId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Subtitle { get; set; } 

    public string? ImageUrl { get; set; }

    public string? PreviewUrl { get; set; }

    public int? ArtistId { get; set; }

    public int? AlbumId { get; set; }
}
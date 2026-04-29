using System;

namespace GrooveOn.Model.ResponseObjects
{
   public class PlayerResponse
{
    public int Id { get; set; }
    public int SongId { get; set; }
    public string Title { get; set; }
    public String ArtistName {get; set;}
    public int Duration {get; set;}
    public string CoverUrl { get; set; }
    public string? PreviewUrl { get; set; }
    public string ExternalTrackId { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}
}

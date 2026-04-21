using System.Collections.Generic;
using GrooveOn.Model.ResponseObjects;

public class MobileHomeResponse
{
    public MusicStatItemResponse? SongOfTheDay { get; set; }
    public List<MusicStatItemResponse> TopTracks { get; set; }
    public List<MusicStatItemResponse> TopArtists { get; set; }
}
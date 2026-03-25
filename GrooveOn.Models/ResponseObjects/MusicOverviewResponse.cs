using System.Collections.Generic;

namespace GrooveOn.Model.ResponseObjects
{
    public class MusicOverviewResponse
    {
        public string Mode { get; set; } = "year";
        public int Year { get; set; }
        public int? Month { get; set; }

        public List<MusicStatItemResponse> MostPlayedSongs { get; set; } 
        public List<MusicStatItemResponse> LeastPlayedSongs { get; set; } 

        public List<MusicStatItemResponse> MostPlayedAlbums { get; set; } 
        public List<MusicStatItemResponse> LeastPlayedAlbums { get; set; } 

        public List<MusicStatItemResponse> MostPlayedArtists { get; set; } 
        public List<MusicStatItemResponse> LeastPlayedArtists { get; set; } 

        public List<GenreStatItemResponse> TrendingGenres { get; set; } 
    }
}
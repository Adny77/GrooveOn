using System.Collections.Generic;

namespace GrooveOn.Model.Responses
{
    public class AlbumPreviewResponse
    {
        public bool AlbumAlreadyExists { get; set; }

        public List<ExistingAlbumTrackResponse> Tracks { get; set; }

        public int ExistingTracksCount { get; set; }
        public int NewTracksCount { get; set; }
    }
}
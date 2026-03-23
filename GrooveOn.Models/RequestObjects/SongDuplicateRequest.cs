using System.Collections.Generic;

namespace GrooveOn.Model.Requests
{
    public class SongDuplicateCheckRequest
    {
        public List<string> ExternalTrackIds { get; set; }
    }
}
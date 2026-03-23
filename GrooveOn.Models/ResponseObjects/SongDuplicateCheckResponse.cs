using System.Collections.Generic;
using System.Linq;

namespace GrooveOn.Model.Responses
{
public class SongDuplicateCheckResponse
    {
        public List<ExistingSongInfoResponse> ExistingSongs { get; set; }
        public List<string> MissingExternalTrackIds { get; set; }
        public bool HasDuplicates => ExistingSongs.Any();
    }
}
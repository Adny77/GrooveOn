using System.Collections.Generic;

namespace GrooveOn.Model.Responses
{
    public class SongBulkInsertResponse
    {
        public int SavedCount { get; set; }
        public List<int> SavedSongIds { get; set; }
    }
}
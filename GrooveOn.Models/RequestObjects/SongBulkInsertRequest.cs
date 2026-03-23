using System.Collections.Generic;

namespace GrooveOn.Model.Requests
{
    public class SongBulkInsertRequest
    {
        public List<SongUpsertRequest> Songs { get; set; }
    }
}
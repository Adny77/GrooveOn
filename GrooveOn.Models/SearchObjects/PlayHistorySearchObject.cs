using System;

namespace GrooveOn.Model.SearchObjects
{
    public class PlayHistorySearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }
        public int? SongId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
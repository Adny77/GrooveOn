using System;

namespace GrooveOn.Model.RequestObjects
{
    public class PlayHistoryUpsertRequest
    {
        public int UserId { get; set; }
        public int SongId { get; set; }

        public DateTime? PlayedAt { get; set; }
    }
}
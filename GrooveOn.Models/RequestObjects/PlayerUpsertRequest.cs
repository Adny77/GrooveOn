using System;

namespace GrooveOn.Model.RequestObjects
{
    public class PlayerUpsertRequest
    {
        public int? PlaylistId {get; set;}
        public int? ArtistId {get; set;}
        public int UserId { get; set; }
        public int SongId { get; set; }
        public String Purpose {get; set;}
    }
}
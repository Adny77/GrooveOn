using System;

namespace GrooveOn.Model.SearchObjects
{
    public class PlayerSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }
        public int? SongId { get; set; }
        public String? Purpose {get; set;}
    }
}
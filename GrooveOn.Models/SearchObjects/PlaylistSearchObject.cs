namespace GrooveOn.Model.SearchObjects
{
    public class PlaylistSearchObject : BaseSearchObject
    {
        public int? ExcludeUserId { get; set;  }
        public int? UserId { get; set; }

        public bool? IsPublic { get; set; }

        public string? FTS { get; set; }
    }
}
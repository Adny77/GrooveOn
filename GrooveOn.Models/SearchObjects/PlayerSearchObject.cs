namespace GrooveOn.Model.SearchObjects
{
    public class PlayerSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }
        public int? SongId { get; set; }
        public bool? IsPlaying { get; set; }
        public bool? IsVisible { get; set; }
    }
}
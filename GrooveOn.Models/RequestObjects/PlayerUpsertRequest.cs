namespace GrooveOn.Model.RequestObjects
{
    public class PlayerUpsertRequest
    {
        public int UserId { get; set; }
        public int SongId { get; set; }
        public int CurrentSeconds { get; set; }
        public bool IsPlaying { get; set; }
        public bool IsVisible { get; set; }
    }
}
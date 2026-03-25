namespace GrooveOn.Model.ResponseObjects
{
    public class MusicStatItemResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int PlayCount { get; set; }
    }
}
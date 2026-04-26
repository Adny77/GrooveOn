namespace GrooveOn.Model.RequestObjects
{
    public class PlaylistUpsertRequest
    {
        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublic { get; set; } = false;

        public string? CoverImageUrl { get; set; }
    }
}
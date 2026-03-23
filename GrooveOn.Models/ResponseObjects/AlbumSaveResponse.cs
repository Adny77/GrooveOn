namespace GrooveOn.Model.Responses
{
    public class AlbumSaveResponse
    {
        public int? AlbumId { get; set; }
        public bool AlbumCreated { get; set; }
        public int SavedTracksCount { get; set; }
        public int ExistingTracksCount { get; set; }
    }
}
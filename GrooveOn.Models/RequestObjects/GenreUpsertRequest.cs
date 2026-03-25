namespace GrooveOn.Model.RequestObjects
{
    public class GenreUpsertRequest
    {
        public string ExternalGenreId { get; set; } = string.Empty;
        public string Source { get; set; } = "Deezer";
        public string Name { get; set; } = string.Empty;
    }
}
using System.Dynamic;

namespace GrooveOn.Model.Requests
{
    public class AlbumGenreUpsertRequest
    {
        public int AlbumId { get; set; }
        public int GenreId { get; set; }
    }
}
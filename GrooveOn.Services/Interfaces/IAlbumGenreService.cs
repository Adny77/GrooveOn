using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IAlbumGenreService
        : ICRUDService<AlbumGenreResponse, AlbumGenreSearchObject, AlbumGenreUpsertRequest, AlbumGenreUpsertRequest>
    {
    }
}
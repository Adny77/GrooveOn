using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IArtistService
    : ICRUDService<ArtistResponse, ArtistSearchObject, ArtistUpsertRequest, ArtistUpsertRequest>
    {
        Task DeleteUnusedArtistsAsync(List<int> artistIds, List<int>? songIdsToIgnore = null);
    }
}
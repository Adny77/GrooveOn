using GrooveOn.Model.Requests;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IAlbumService
    : ICRUDService<AlbumResponse, BaseSearchObject, AlbumUpsertRequest, AlbumUpsertRequest>
{
    Task<AlbumPreviewResponse> PreviewDeezerAlbumAsync(AlbumUpsertRequest request);
    Task<AlbumSaveResponse> SaveDeezerAlbumAsync(AlbumUpsertRequest request);
}
}
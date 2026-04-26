using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IPlaylistSongService
        : ICRUDService<PlaylistSongResponse, PlaylistSongSearchObject, PlaylistSongUpsertRequest, PlaylistSongUpsertRequest>
    {
    }
}
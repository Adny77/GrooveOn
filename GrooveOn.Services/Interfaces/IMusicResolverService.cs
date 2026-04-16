using GrooveOn.Model.RequestObjects;
using GrooveOn.Services.Database;

namespace GrooveOn.Services.Interfaces
{
    public interface IMusicResolveService
    {
        Task<Artist> ResolveArtistAsync(ResolveArtistRequest request);
        Task<Album?> ResolveAlbumAsync(ResolveAlbumRequest request, Artist artist);
    }
}
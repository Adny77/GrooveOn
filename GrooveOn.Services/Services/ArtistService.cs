using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class ArtistService
        : BaseCRUDService<ArtistResponse, ArtistSearchObject, Artist, ArtistUpsertRequest, ArtistUpsertRequest>, IArtistService
    {
        public ArtistService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task DeleteUnusedArtistsAsync(List<int> artistIds, List<int>? songIdsToIgnore = null)
        {
            if (artistIds == null || !artistIds.Any())
                return;

            var distinctArtistIds = artistIds.Distinct().ToList();
            var ignoredSongIds = songIdsToIgnore?.Distinct().ToList() ?? new List<int>();

            foreach (var artistId in distinctArtistIds)
            {
                var query = _context.Songs
                    .Where(x => x.ArtistId == artistId);

                if (ignoredSongIds.Any())
                    query = query.Where(x => !ignoredSongIds.Contains(x.Id));

                var artistStillUsed = await query.AnyAsync();

                if (!artistStillUsed)
                {
                    var artist = await _context.Artists.FirstOrDefaultAsync(x => x.Id == artistId);
                    if (artist != null)
                        _context.Artists.Remove(artist);
                }
            }
        }
    }
}
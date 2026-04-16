using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class PlayHistoryService
        : BaseCRUDService<PlayHistoryResponse, PlayHistorySearchObject, PlayHistory, PlayHistoryUpsertRequest, PlayHistoryUpsertRequest>, IPlayHistoryService
    {
        public PlayHistoryService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        public async Task DeleteBySongIdsAsync(List<int> songIds, List<int>? songIdsToIgnore = null)
        {
            if (songIds == null || !songIds.Any())
                return;

            var distinctSongIds = songIds.Distinct().ToList();
            var ignoredSongIds = songIdsToIgnore?.Distinct().ToList() ?? new List<int>();

            var query = _context.PlayHistories
                .Where(x => distinctSongIds.Contains(x.SongId));

            if (ignoredSongIds.Any())
                query = query.Where(x => !ignoredSongIds.Contains(x.SongId));

            var playHistories = await query.ToListAsync();

            if (playHistories.Any())
                _context.PlayHistories.RemoveRange(playHistories);
        }
    }
}
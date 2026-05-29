using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace GrooveOn.Services.Services
{
    public class MusicSearchEngineService 
        : BaseService<MusicSearchItemResponse, MusicSearchSearchObject, Song>, IMusicSearchEngineService
    {
        private readonly GrooveOnDbContext _context;

        public MusicSearchEngineService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
        }

        public override async Task<PagedResult<MusicSearchItemResponse>> GetAsync(MusicSearchSearchObject search)
        {
            var fts = search.FTS?.Trim().ToLower();

            // Cap per-type results at DB level to avoid loading entire tables into memory.
            var perTypeCap = search.RetrieveAll ? 200 : Math.Max((search.PageSize ?? 20) * 4, 60);

            var songsQuery = _context.Songs
                .Include(x => x.Artist)
                .Include(x => x.Album)
                .AsQueryable();

            var albumsQuery = _context.Albums
                .Include(x => x.Artist)
                .AsQueryable();

            var artistsQuery = _context.Artists
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(fts))
            {
                songsQuery = songsQuery.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    (x.Artist != null && x.Artist.Name.ToLower().Contains(fts)) ||
                    (x.Album != null && x.Album.Title.ToLower().Contains(fts))
                );

                albumsQuery = albumsQuery.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    (x.Artist != null && x.Artist.Name.ToLower().Contains(fts))
                );

                artistsQuery = artistsQuery.Where(x =>
                    x.Name.ToLower().Contains(fts)
                );
            }

            var songItems = await songsQuery
                .OrderBy(x => x.Id)
                .Take(perTypeCap)
                .Select(x => new MusicSearchItemResponse
                {
                    Type = "song",
                    Id = x.Id,
                    ExternalTrackId = x.ExternalTrackId,
                    Title = x.Title,
                    Subtitle = x.Artist != null ? x.Artist.Name : null,
                    ImageUrl = x.CoverUrl,
                    PreviewUrl = x.PreviewUrl,
                    ArtistId = x.ArtistId,
                    AlbumId = x.AlbumId
                })
                .ToListAsync();

            var albumItems = await albumsQuery
                .OrderBy(x => x.Id)
                .Take(perTypeCap)
                .Select(x => new MusicSearchItemResponse
                {
                    Type = "album",
                    Id = x.Id,
                    Title = x.Title,
                    Subtitle = x.Artist != null ? x.Artist.Name : null,
                    ImageUrl = x.CoverUrl,
                    PreviewUrl = null,
                    ArtistId = x.ArtistId,
                    AlbumId = x.Id
                })
                .ToListAsync();

            var artistItems = await artistsQuery
                .OrderBy(x => x.Id)
                .Take(perTypeCap)
                .Select(x => new MusicSearchItemResponse
                {
                    Type = "artist",
                    Id = x.Id,
                    Title = x.Name,
                    Subtitle = "Artist",
                    ImageUrl = x.Picture,
                    PreviewUrl = null,
                    ArtistId = x.Id,
                    AlbumId = null
                })
                .ToListAsync();

            var combined = songItems
                .Concat(albumItems)
                .Concat(artistItems)
                .OrderBy(x => x.Type == "song" ? 0 : x.Type == "album" ? 1 : 2)
                .ThenBy(x => x.Title)
                .ThenBy(x => x.Id)
                .ToList();

            int? totalCount = null;
            if (search.IncludeTotalCount)
            {
                totalCount = combined.Count;
            }

            if (!search.RetrieveAll)
            {
                var pageSize = search.PageSize ?? 20;
                var page = search.Page ?? 0;

                combined = combined
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToList();
            }

            return new PagedResult<MusicSearchItemResponse>
            {
                Items = combined,
                TotalCount = totalCount
            };
        }
    }
}

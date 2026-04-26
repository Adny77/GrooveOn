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
                .ToList();

            int? totalCount = null;
            if (search.IncludeTotalCount)
            {
                totalCount = combined.Count;
            }

            if (!search.RetrieveAll)
            {
                if (search.Page.HasValue)
                {
                    combined = combined
                        .Skip(search.Page.Value * search.PageSize.Value)
                        .ToList();
                }

                if (search.PageSize.HasValue)
                {
                    combined = combined
                        .Take(search.PageSize.Value)
                        .ToList();
                }
            }

            return new PagedResult<MusicSearchItemResponse>
            {
                Items = combined,
                TotalCount = totalCount
            };
        }
    }
}
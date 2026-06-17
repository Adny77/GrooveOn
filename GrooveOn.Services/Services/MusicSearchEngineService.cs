using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

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

            var songsQuery = _context.Songs.AsQueryable();
            var albumsQuery = _context.Albums.AsQueryable();
            var artistsQuery = _context.Artists.AsQueryable();

            if (!string.IsNullOrWhiteSpace(fts))
            {
                songsQuery = songsQuery.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    (x.Artist != null && x.Artist.Name.ToLower().Contains(fts)) ||
                    (x.Album != null && x.Album.Title.ToLower().Contains(fts)));

                albumsQuery = albumsQuery.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    (x.Artist != null && x.Artist.Name.ToLower().Contains(fts)));

                artistsQuery = artistsQuery.Where(x =>
                    x.Name.ToLower().Contains(fts));
            }

            // All three COUNT queries run concurrently at DB level.
            var songCountTask = songsQuery.CountAsync();
            var albumCountTask = albumsQuery.CountAsync();
            var artistCountTask = artistsQuery.CountAsync();

            await Task.WhenAll(songCountTask, albumCountTask, artistCountTask);

            var songCount = songCountTask.Result;
            var albumCount = albumCountTask.Result;
            var artistCount = artistCountTask.Result;
            var totalCount = songCount + albumCount + artistCount;

            List<MusicSearchItemResponse> items;

            if (search.RetrieveAll)
            {
                var songItems = await BuildSongQuery(songsQuery).ToListAsync();
                var albumItems = await BuildAlbumQuery(albumsQuery).ToListAsync();
                var artistItems = await BuildArtistQuery(artistsQuery).ToListAsync();

                items = [.. songItems, .. albumItems, .. artistItems];
            }
            else
            {
                var pageSize = search.PageSize ?? 20;
                var page = search.Page ?? 0;
                var globalSkip = page * pageSize;
                var globalTake = pageSize;

                items = await FetchPageAsync(
                    songsQuery, albumsQuery, artistsQuery,
                    songCount, albumCount, artistCount,
                    globalSkip, globalTake);
            }

            return new PagedResult<MusicSearchItemResponse>
            {
                Items = items,
                TotalCount = search.IncludeTotalCount ? totalCount : null
            };
        }

        // Fetches only the rows needed for the requested page window.
        // The virtual ordering is: songs first, albums second, artists third.
        // Each type is ordered by Title then Id.
        private async Task<List<MusicSearchItemResponse>> FetchPageAsync(
            IQueryable<Song> songsQuery,
            IQueryable<Album> albumsQuery,
            IQueryable<Artist> artistsQuery,
            int songCount, int albumCount, int artistCount,
            int globalSkip, int globalTake)
        {
            var result = new List<MusicSearchItemResponse>(globalTake);

            // Songs occupy global positions [0, songCount).
            var songStart = Math.Max(0, globalSkip);
            var songEnd = Math.Min(songCount, globalSkip + globalTake);
            if (songStart < songEnd)
            {
                var slice = await BuildSongQuery(songsQuery)
                    .Skip(songStart)
                    .Take(songEnd - songStart)
                    .ToListAsync();
                result.AddRange(slice);
            }

            // Albums occupy global positions [songCount, songCount + albumCount).
            var albumGlobalStart = songCount;
            var albumStart = Math.Max(0, globalSkip - albumGlobalStart);
            var albumEnd = Math.Min(albumCount, globalSkip + globalTake - albumGlobalStart);
            if (albumStart < albumEnd)
            {
                var slice = await BuildAlbumQuery(albumsQuery)
                    .Skip(albumStart)
                    .Take(albumEnd - albumStart)
                    .ToListAsync();
                result.AddRange(slice);
            }

            // Artists occupy global positions [songCount + albumCount, total).
            var artistGlobalStart = songCount + albumCount;
            var artistStart = Math.Max(0, globalSkip - artistGlobalStart);
            var artistEnd = Math.Min(artistCount, globalSkip + globalTake - artistGlobalStart);
            if (artistStart < artistEnd)
            {
                var slice = await BuildArtistQuery(artistsQuery)
                    .Skip(artistStart)
                    .Take(artistEnd - artistStart)
                    .ToListAsync();
                result.AddRange(slice);
            }

            return result;
        }

        private static IQueryable<MusicSearchItemResponse> BuildSongQuery(IQueryable<Song> q) =>
            q.OrderBy(x => x.Title).ThenBy(x => x.Id)
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
             });

        private static IQueryable<MusicSearchItemResponse> BuildAlbumQuery(IQueryable<Album> q) =>
            q.OrderBy(x => x.Title).ThenBy(x => x.Id)
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
             });

        private static IQueryable<MusicSearchItemResponse> BuildArtistQuery(IQueryable<Artist> q) =>
            q.OrderBy(x => x.Name).ThenBy(x => x.Id)
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
             });
    }
}

using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class AlbumService
        : BaseCRUDService<AlbumResponse, BaseSearchObject, Album, AlbumUpsertRequest, AlbumUpsertRequest>,
          IAlbumService
    {
        private readonly GrooveOnDbContext _context;

        public AlbumService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
        }

        protected override IQueryable<Album> AddInclude(IQueryable<Album> query, BaseSearchObject search = null!)
        {
            return query
                .Include(x => x.Artist)
                .Include(x => x.Songs);
        }

        protected override IQueryable<Album> ApplyFilter(IQueryable<Album> query, BaseSearchObject search)
        {
            query = base.ApplyFilter(query, search);

            if (!string.IsNullOrWhiteSpace(search?.FTS))
            {
                var fts = search.FTS.ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    (x.Artist != null && x.Artist.Name.ToLower().Contains(fts))
                );
            }

            return query;
        }

        public async Task<AlbumPreviewResponse> PreviewDeezerAlbumAsync(AlbumUpsertRequest request)
        {
            var albumAlreadyExists = await _context.Albums
                .AnyAsync(x =>
                    x.ExternalAlbumId != null &&
                    x.ExternalAlbumId == request.ExternalAlbumId
                );

            var externalTrackIds = request.Tracks
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
                .Select(x => x.ExternalTrackId)
                .Distinct()
                .ToList();

            var existingTrackIds = await _context.Songs
                .Where(x =>
                    x.ExternalTrackId != null &&
                    externalTrackIds.Contains(x.ExternalTrackId))
                .Select(x => x.ExternalTrackId!)
                .ToListAsync();

            var existingTrackSet = existingTrackIds.ToHashSet();

            var tracks = request.Tracks
                .Select(x => new ExistingAlbumTrackResponse
                {
                    ExternalTrackId = x.ExternalTrackId,
                    Title = x.Title,
                    AlreadyExists = existingTrackSet.Contains(x.ExternalTrackId)
                })
                .ToList();

            return new AlbumPreviewResponse
            {
                AlbumAlreadyExists = albumAlreadyExists,
                Tracks = tracks,
                ExistingTracksCount = tracks.Count(x => x.AlreadyExists),
                NewTracksCount = tracks.Count(x => !x.AlreadyExists)
            };
        }

        public async Task<AlbumSaveResponse> SaveDeezerAlbumAsync(AlbumUpsertRequest request)
{
    if (string.IsNullOrWhiteSpace(request.ExternalAlbumId))
        throw new Exception("ExternalAlbumId is required.");

    if (string.IsNullOrWhiteSpace(request.Title))
        throw new Exception("Album title is required.");

    if (string.IsNullOrWhiteSpace(request.ArtistName))
        throw new Exception("Artist name is required.");

    Artist? artist = null;

    if (!string.IsNullOrWhiteSpace(request.ExternalArtistId))
    {
        artist = await _context.Artists
            .FirstOrDefaultAsync(x =>
                x.ExternalArtistId != null &&
                x.ExternalArtistId == request.ExternalArtistId);
    }

    if (artist == null)
    {
        artist = await _context.Artists
            .FirstOrDefaultAsync(x => x.Name == request.ArtistName);
    }

    if (artist == null)
    {
        artist = new Artist
        {
            ExternalArtistId = request.ExternalArtistId,
            Name = request.ArtistName,
            ImageUrl = request.CoverUrl,
            CreatedAt = DateTime.UtcNow
        };

        _context.Artists.Add(artist);
        await _context.SaveChangesAsync();
    }
    else
    {
        if (string.IsNullOrWhiteSpace(artist.ExternalArtistId) &&
            !string.IsNullOrWhiteSpace(request.ExternalArtistId))
        {
            artist.ExternalArtistId = request.ExternalArtistId;
        }

        if (string.IsNullOrWhiteSpace(artist.ImageUrl) &&
            !string.IsNullOrWhiteSpace(request.CoverUrl))
        {
            artist.ImageUrl = request.CoverUrl;
        }

        await _context.SaveChangesAsync();
    }

    var album = await _context.Albums
        .Include(x => x.Songs)
        .FirstOrDefaultAsync(x =>
            x.ExternalAlbumId != null &&
            x.ExternalAlbumId == request.ExternalAlbumId);

    if (album == null)
    {
        album = await _context.Albums
            .Include(x => x.Songs)
            .FirstOrDefaultAsync(x =>
                x.Title == request.Title &&
                x.ArtistId == artist.Id);
    }

    var albumCreated = false;

    if (album == null)
    {
        album = new Album
        {
            ExternalAlbumId = request.ExternalAlbumId,
            Title = request.Title,
            ArtistId = artist.Id,
            ReleaseDate = request.ReleaseDate,
            CoverUrl = request.CoverUrl,
            CreatedAt = DateTime.UtcNow
        };

        _context.Albums.Add(album);
        await _context.SaveChangesAsync();

        albumCreated = true;
    }
    else
    {
        if (string.IsNullOrWhiteSpace(album.ExternalAlbumId) &&
            !string.IsNullOrWhiteSpace(request.ExternalAlbumId))
        {
            album.ExternalAlbumId = request.ExternalAlbumId;
        }

        album.Title = request.Title;
        album.ArtistId = artist.Id;
        album.ReleaseDate = request.ReleaseDate;
        album.CoverUrl = request.CoverUrl;

        await _context.SaveChangesAsync();
    }

    var externalTrackIds = request.Tracks
        .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
        .Select(x => x.ExternalTrackId)
        .Distinct()
        .ToList();

    var existingSongs = await _context.Songs
        .Where(x =>
            x.ExternalTrackId != null &&
            externalTrackIds.Contains(x.ExternalTrackId))
        .ToListAsync();

    var existingSongMap = existingSongs
        .Where(x => !string.IsNullOrWhiteSpace(x.ExternalTrackId))
        .ToDictionary(x => x.ExternalTrackId!, x => x);

    var existingTracksCount = existingSongMap.Count;
    var savedTracksCount = 0;

    foreach (var track in request.Tracks)
    {
        if (string.IsNullOrWhiteSpace(track.ExternalTrackId))
            continue;

        if (existingSongMap.TryGetValue(track.ExternalTrackId, out var existingSong))
        {
            if (existingSong.AlbumId == null)
            {
                existingSong.AlbumId = album.Id;
            }

            if (existingSong.ArtistId != artist.Id)
            {
                existingSong.ArtistId = artist.Id;
            }

            if (string.IsNullOrWhiteSpace(existingSong.CoverUrl) &&
                !string.IsNullOrWhiteSpace(track.CoverUrl ?? request.CoverUrl))
            {
                existingSong.CoverUrl = track.CoverUrl ?? request.CoverUrl;
            }

            if (existingSong.ReleaseDate == null)
            {
                existingSong.ReleaseDate = track.ReleaseDate ?? request.ReleaseDate;
            }

            if (string.IsNullOrWhiteSpace(existingSong.PreviewUrl) &&
                !string.IsNullOrWhiteSpace(track.PreviewUrl))
            {
                existingSong.PreviewUrl = track.PreviewUrl;
            }

            existingSong.LastSyncedAt = DateTime.UtcNow;
        }
        else
        {
            var entity = new Song
            {
                ExternalTrackId = track.ExternalTrackId,
                Source = track.Source,
                Title = track.Title,
                ArtistId = artist.Id,
                AlbumId = album.Id,
                DurationSeconds = track.DurationSeconds,
                PreviewUrl = track.PreviewUrl,
                CoverUrl = track.CoverUrl ?? request.CoverUrl,
                ReleaseDate = track.ReleaseDate ?? request.ReleaseDate,
                IsActive = true,
                LastSyncedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.Songs.Add(entity);
            savedTracksCount++;
        }
    }

    await _context.SaveChangesAsync();

    return new AlbumSaveResponse
    {
        AlbumId = album.Id,
        AlbumCreated = albumCreated,
        SavedTracksCount = savedTracksCount,
        ExistingTracksCount = existingTracksCount
    };
}
    }
}
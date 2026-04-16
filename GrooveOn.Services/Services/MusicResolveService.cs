using GrooveOn.Model.RequestObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class MusicResolveService : IMusicResolveService
    {
        private readonly GrooveOnDbContext _context;

        public MusicResolveService(GrooveOnDbContext context)
        {
            _context = context;
        }

        public async Task<Artist> ResolveArtistAsync(ResolveArtistRequest request)
        {
            Artist? artist = null;

            if (!string.IsNullOrWhiteSpace(request.ExternalArtistId))
            {
                artist = await _context.Artists
                    .FirstOrDefaultAsync(x => x.ExternalArtistId == request.ExternalArtistId);
            }

            if (artist == null && !string.IsNullOrWhiteSpace(request.ArtistName))
            {
                artist = await _context.Artists
                    .FirstOrDefaultAsync(x => x.Name == request.ArtistName);
            }

            if (artist == null)
            {
                artist = new Artist
                {
                    ExternalArtistId = request.ExternalArtistId,
                    Source = string.IsNullOrWhiteSpace(request.Source) ? "Deezer" : request.Source,
                    Name = request.ArtistName ?? "Unknown artist",
                    Picture = request.ArtistPicture,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Artists.Add(artist);
                await _context.SaveChangesAsync();
            }
            else
            {
                var changed = false;

                if (string.IsNullOrWhiteSpace(artist.ExternalArtistId) &&
                    !string.IsNullOrWhiteSpace(request.ExternalArtistId))
                {
                    artist.ExternalArtistId = request.ExternalArtistId;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(artist.Source) &&
                    !string.IsNullOrWhiteSpace(request.Source))
                {
                    artist.Source = request.Source;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(artist.Picture) &&
                    !string.IsNullOrWhiteSpace(request.ArtistPicture))
                {
                    artist.Picture = request.ArtistPicture;
                    changed = true;
                }

                if (changed)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return artist;
        }

        public async Task<Album?> ResolveAlbumAsync(ResolveAlbumRequest request, Artist artist)
        {
            if (request.AllowNull &&
                string.IsNullOrWhiteSpace(request.Title) &&
                string.IsNullOrWhiteSpace(request.ExternalAlbumId))
            {
                return null;
            }

            IQueryable<Album> query = _context.Albums;

            if (request.IncludeDetails)
            {
                query = query
                    .Include(x => x.Songs)
                    .Include(x => x.AlbumGenres);
            }

            Album? album = null;

            if (!string.IsNullOrWhiteSpace(request.ExternalAlbumId))
            {
                album = await query
                    .FirstOrDefaultAsync(x => x.ExternalAlbumId == request.ExternalAlbumId);
            }

            if (album == null && !string.IsNullOrWhiteSpace(request.Title))
            {
                album = await query
                    .FirstOrDefaultAsync(x =>
                        x.Title == request.Title &&
                        x.ArtistId == artist.Id);
            }

            if (album == null)
            {
                album = new Album
                {
                    ExternalAlbumId = request.ExternalAlbumId,
                    Source = string.IsNullOrWhiteSpace(request.Source) ? "Deezer" : request.Source,
                    Title = request.Title ?? "Unknown album",
                    ArtistId = artist.Id,
                    ReleaseDate = request.ReleaseDate,
                    CoverUrl = request.CoverUrl,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Albums.Add(album);
                await _context.SaveChangesAsync();
            }
            else
            {
                var changed = false;

                if (string.IsNullOrWhiteSpace(album.ExternalAlbumId) &&
                    !string.IsNullOrWhiteSpace(request.ExternalAlbumId))
                {
                    album.ExternalAlbumId = request.ExternalAlbumId;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.Source) &&
                    !string.IsNullOrWhiteSpace(request.Source))
                {
                    album.Source = request.Source;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.CoverUrl) &&
                    !string.IsNullOrWhiteSpace(request.CoverUrl))
                {
                    album.CoverUrl = request.CoverUrl;
                    changed = true;
                }

                if (!album.ReleaseDate.HasValue && request.ReleaseDate.HasValue)
                {
                    album.ReleaseDate = request.ReleaseDate;
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(album.Description) &&
                    !string.IsNullOrWhiteSpace(request.Description))
                {
                    album.Description = request.Description;
                    changed = true;
                }

                if (changed)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return album;
        }
    }
}
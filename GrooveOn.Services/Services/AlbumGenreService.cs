using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class AlbumGenreService
        : BaseCRUDService<AlbumGenreResponse, AlbumGenreSearchObject, AlbumGenre, AlbumGenreUpsertRequest, AlbumGenreUpsertRequest>,
          IAlbumGenreService
    {

        private readonly IGenreService _genreService;

        public AlbumGenreService(GrooveOnDbContext context, IMapper mapper, IGenreService genreService)
            : base(context, mapper)
        {
            _genreService = genreService;
        }

        protected override IQueryable<AlbumGenre> AddInclude(IQueryable<AlbumGenre> query, AlbumGenreSearchObject? search = null)
        {
            return query
                .Include(x => x.Album)
                .Include(x => x.Genre);
        }

        protected override IQueryable<AlbumGenre> ApplyFilter(IQueryable<AlbumGenre> query, AlbumGenreSearchObject search)
        {
            query = base.ApplyFilter(query, search);

            if (search.AlbumId.HasValue)
            {
                query = query.Where(x => x.AlbumId == search.AlbumId.Value);
            }

            if (search.GenreId.HasValue)
            {
                query = query.Where(x => x.GenreId == search.GenreId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.Trim().ToLower();

                query = query.Where(x =>
                    x.Album.Title.ToLower().Contains(fts) ||
                    x.Genre.Name.ToLower().Contains(fts)
                );
            }

            return query;
        }

        protected override async Task BeforeInsert(AlbumGenre entity, AlbumGenreUpsertRequest insert)
        {
            await ValidateDuplicateAsync(insert.AlbumId, insert.GenreId, null);
            await base.BeforeInsert(entity, insert);
        }

        protected override async Task BeforeUpdate(AlbumGenre entity, AlbumGenreUpsertRequest update)
        {
            await ValidateDuplicateAsync(update.AlbumId, update.GenreId, entity.Id);
            await base.BeforeUpdate(entity, update);
        }

        public async Task DeleteByAlbumIdAsync(int albumId, int? albumIdToIgnore = null)
        {
            var query = _context.AlbumGenres
                .Where(x => x.AlbumId == albumId);

            if (albumIdToIgnore.HasValue)
                query = query.Where(x => x.AlbumId != albumIdToIgnore.Value);

            var albumGenres = await query.ToListAsync();

            if (albumGenres.Any())
                _context.AlbumGenres.RemoveRange(albumGenres);
        }

        public async Task SaveAlbumGenresAsync(int albumId, List<GenreUpsertRequest> genres)
        {
            if (genres == null || !genres.Any())
                return;

            foreach (var item in genres
                .Where(x => !string.IsNullOrWhiteSpace(x.ExternalGenreId) && !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => x.ExternalGenreId)
                .Select(g => g.First()))
            {

                var genre = await _genreService.GetByExternalGenreAsync(item.ExternalGenreId, item.Source);

                if (genre == null)
                {
                    await _genreService.CreateAsync(item);
                }
                else if (string.IsNullOrWhiteSpace(genre.Name))
                {
                    await _genreService.UpdateAsync(genre.Id, item);
                }
            }
        }
        private async Task ValidateDuplicateAsync(int albumId, int genreId, int? ignoreId)
        {
            var query = _context.AlbumGenres
                .Where(x => x.AlbumId == albumId && x.GenreId == genreId);

            if (ignoreId.HasValue)
            {
                query = query.Where(x => x.Id != ignoreId.Value);
            }

            var exists = await query.AnyAsync();

            if (exists)
            {
                throw new InvalidOperationException("The relationship between the album and genre already exists.");
            }
        }
    }
}
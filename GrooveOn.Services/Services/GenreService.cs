using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class GenreService : BaseCRUDService<GenreResponse, GenreSearchObject, Genre, GenreUpsertRequest, GenreUpsertRequest>, IGenreService
    {
        private readonly GrooveOnDbContext _context;
        private readonly IMapper _mapper;

        public GenreService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override IQueryable<Genre> ApplyFilter(IQueryable<Genre> query, GenreSearchObject search)
        {
            query = base.ApplyFilter(query, search);

            if (!string.IsNullOrWhiteSpace(search.Name))
                query = query.Where(x => x.Name.Contains(search.Name));

            if (!string.IsNullOrWhiteSpace(search.ExternalGenreId))
                query = query.Where(x => x.ExternalGenreId == search.ExternalGenreId);

            if (!string.IsNullOrWhiteSpace(search.Source))
                query = query.Where(x => x.Source == search.Source);
            return base.ApplyFilter(query, search);
        }

        public async Task<GenreResponse?> GetByExternalGenreAsync(string externalGenreId, string source)
        {
            var entity = await _context.Genres
                .FirstOrDefaultAsync(x => x.ExternalGenreId == externalGenreId && x.Source == source);

            return entity == null ? null : _mapper.Map<GenreResponse>(entity);
        }

        public async Task DeleteUnusedGenresAsync(List<int> genreIds, int? albumIdToIgnore = null)
        {
            if (genreIds == null || !genreIds.Any())
                return;

            var distinctGenreIds = genreIds.Distinct().ToList();

            foreach (var genreId in distinctGenreIds)
            {
                var query = _context.AlbumGenres
                    .Where(x => x.GenreId == genreId);

                if (albumIdToIgnore.HasValue)
                    query = query.Where(x => x.AlbumId != albumIdToIgnore.Value);

                var genreStillUsed = await query.AnyAsync();

                if (!genreStillUsed)
                {
                    var genre = await _context.Genres.FirstOrDefaultAsync(x => x.Id == genreId);
                    if (genre != null)
                        _context.Genres.Remove(genre);
                }
            }
        }


        protected override async Task BeforeInsert(Genre entity, GenreUpsertRequest insert)
        {
            var exists = await _context.Genres.AnyAsync(x =>
                x.ExternalGenreId == insert.ExternalGenreId &&
                x.Source == insert.Source);

            if (exists)
                throw new InvalidOperationException("Genre already exists");

            entity.CreatedAt = DateTime.UtcNow;

            await base.BeforeInsert(entity, insert);
        }
    }
}
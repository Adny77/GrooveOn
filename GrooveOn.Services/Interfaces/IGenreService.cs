using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IGenreService : ICRUDService<GenreResponse, GenreSearchObject, GenreUpsertRequest, GenreUpsertRequest>
    {
        public Task<GenreResponse?> GetByExternalGenreAsync(string externalGenreId, string source);

        public Task DeleteUnusedGenresAsync(List<int> genreIds, int? albumIdToIgnore = null);
    }
}
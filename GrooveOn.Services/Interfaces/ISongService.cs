using GrooveOn.Model.Requests;
using GrooveOn.Model.Responses;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;

namespace GrooveOn.Services.Interfaces
{
    public interface ISongService 
        : ICRUDService<SongResponse, SongSearchObject, SongUpsertRequest, SongUpsertRequest>
    {
        Task<SongDuplicateCheckResponse> CheckDuplicatesAsync(SongDuplicateCheckRequest request);

        Task<SongBulkInsertResponse> BulkInsertDeezerSongsAsync(SongBulkInsertRequest request);

        Task<List<SongResponse>> GetRecommendedForUserAsync(int userId, int take = 4);
    }
}
using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObject;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IPlayHistoryService
    : ICRUDService<PlayHistoryResponse, PlayHistorySearchObject, PlayHistoryUpsertRequest, PlayHistoryUpsertRequest>
    {
        Task DeleteBySongIdsAsync(List<int> songIds, List<int>? songIdsToIgnore = null);
    }
}
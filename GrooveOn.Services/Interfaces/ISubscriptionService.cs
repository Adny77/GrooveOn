using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface ISubscriptionService
        : ICRUDService<SubscriptionResponse, SubscriptionSearchObject, SubscriptionUpsertRequest, SubscriptionUpsertRequest>
    {
        Task<SubscriptionResponse?> GetActiveByUserIdAsync(int userId);
    }
}
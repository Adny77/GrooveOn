using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Models.SearchObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface INotificationService
        : ICRUDService<NotificationResponse, NotificationSearchObject, NotificationUpsertRequest, NotificationUpsertRequest>
    {
        Task<NotificationResponse?> MarkAsReadAsync(int id, int userId);
        void AddForUser(int userId, string title, string content, string type);
    }
}

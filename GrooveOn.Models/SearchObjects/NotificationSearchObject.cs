using GrooveOn.Model.SearchObjects;

namespace GrooveOn.Models.SearchObjects
{
    public class NotificationSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }
        public bool? IsRead { get; set; }
        public string? Type { get; set; }
    }
}

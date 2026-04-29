namespace GrooveOn.Model.SearchObjects
{
    public class SubscriptionSearchObject : BaseSearchObject
    {
        public int? UserId { get; set; }

        public int? SubscriptionPlanId { get; set; }

        public bool? IsActive { get; set; }

        public bool? OnlyExpired { get; set; }

        public bool? OnlyValid { get; set; }
    }
}
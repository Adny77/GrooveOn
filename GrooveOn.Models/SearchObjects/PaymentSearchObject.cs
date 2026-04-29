namespace GrooveOn.Model.SearchObjects
{
    public class PaymentSearchObject : BaseSearchObject
    {
        public int? SubscriptionId { get; set; }

        public int? UserId { get; set; }

        public string? PaymentStatus { get; set; }

        public string? StripePaymentIntentId { get; set; }
    }
}
using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class PaymentResponse
    {
        public int Id { get; set; }

        public int SubscriptionId { get; set; }
        public int? SubscriptionPlanId { get; set; }
        public string? SubscriptionPlanName { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string? StripePaymentIntentId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public string? FailureReason { get; set; }

        public int? UserId { get; set; }
        public string? Username { get; set; }
    }
}
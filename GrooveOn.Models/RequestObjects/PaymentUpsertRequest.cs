using System;
using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Model.RequestObjects
{
    public class PaymentUpsertRequest
    {
        [Required]
        public int SubscriptionId { get; set; }

        public string PaymentStatus { get; set; } = "Pending";

        public string? StripePaymentIntentId { get; set; }

        public DateTime? PaidAt { get; set; }

        public string? FailureReason { get; set; }
    }
}
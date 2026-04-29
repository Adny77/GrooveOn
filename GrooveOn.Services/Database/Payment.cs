using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Payment
    {
        public int Id { get; set; }

        [ForeignKey(nameof(SubscriptionId))]
        public int SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }
        public string PaymentStatus { get; set; } = "Pending";
        public string? StripePaymentIntentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public string? FailureReason { get; set; }
        public string? PaymentMethod { get; set; }
        public float PaymentAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
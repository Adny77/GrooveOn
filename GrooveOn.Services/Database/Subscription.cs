using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Subscription
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }

        public User? User { get; set; }

        [ForeignKey(nameof(SubscriptionPlanId))]
        public int SubscriptionPlanId { get; set; }

        public SubscriptionPlan? SubscriptionPlan { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string? PaymentMethod { get; set; }

        public float PaymentAmount { get; set; }

        public DateTime? PaymentDate { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrooveOn.Services.Database
{
    public class Subscriber
    {
        [Key]
        public int SubscriptionId { get; set; }

        public bool SubscriptionStatus { get; set; }

        [ForeignKey(nameof(UserId))]
        public int UserId { get; set; }

        public User? User { get; set; }
        public string SubscriptionType { get; set; } = string.Empty;

        public DateTime SubscriptionStartDate { get; set; }

        public DateTime SubscriptionExpiryDate { get; set; }

        public string? PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        public float PaymentAmount { get; set; }
    }
}
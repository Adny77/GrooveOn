using System;

namespace GrooveOn.Model.ResponseObjects
{
    public class SubscriptionResponse
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public string? UserFullName { get; set; }

        public int SubscriptionPlanId { get; set; }

        public string? SubscriptionPlanName { get; set; }

        public float SubscriptionPlanPrice { get; set; }

        public int SubscriptionPlanDurationDays { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsExpired =>
            ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
    }
}
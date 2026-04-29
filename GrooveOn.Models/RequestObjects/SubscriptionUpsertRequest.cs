using System;
using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Model.RequestObjects
{
    public class SubscriptionUpsertRequest
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int SubscriptionPlanId { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
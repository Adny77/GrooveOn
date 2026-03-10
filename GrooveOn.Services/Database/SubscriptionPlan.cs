using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Services.Database
{
    public class SubscriptionPlan
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public float Price { get; set; }

        public int DurationDays { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
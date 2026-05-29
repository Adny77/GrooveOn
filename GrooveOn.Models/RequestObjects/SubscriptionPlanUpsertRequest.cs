using System.ComponentModel.DataAnnotations;

namespace GrooveOn.Model.RequestObjects
{
    public class SubscriptionPlanUpsertRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string PlanCode { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int DurationDays { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}

namespace GrooveOn.Model.ResponseObjects
{
    public class SubscriptionPlanResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string PlanCode { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int DurationDays { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}

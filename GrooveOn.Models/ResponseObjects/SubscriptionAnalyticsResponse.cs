namespace GrooveOn.Model.ResponseObjects
{
    public class SubscriptionAnalyticsResponse
    {
        public int BasicCount { get; set; }
        public int PremiumCount { get; set; }
        public double BasicPercentage { get; set; }
        public double PremiumPercentage { get; set; }
        public int TotalCount { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;
    }
}
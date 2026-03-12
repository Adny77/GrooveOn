using GrooveOn.Model.ResponseObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly GrooveOnDbContext _context;

        public ReportService(GrooveOnDbContext context)
        {
            _context = context;
        }

        public SubscriptionAnalyticsResponse GetSubscriptionAnalytics(int year, int? month = null)
        {
            var query = _context.Subscriptions.AsQueryable();

            query = query.Where(x => x.StartDate.Year == year);

            if (month.HasValue)
            {
                query = query.Where(x => x.StartDate.Month == month.Value);
            }

            var basicCount = query.Count(x => x.SubscriptionPlanId == 1);
            var premiumCount = query.Count(x => x.SubscriptionPlanId == 2 || x.SubscriptionPlanId == 3);

            var total = basicCount + premiumCount;

            double basicPercentage = total == 0 ? 0 : (double)basicCount / total * 100;
            double premiumPercentage = total == 0 ? 0 : (double)premiumCount / total * 100;

            return new SubscriptionAnalyticsResponse
            {
                BasicCount = basicCount,
                PremiumCount = premiumCount,
                BasicPercentage = Math.Round(basicPercentage, 2),
                PremiumPercentage = Math.Round(premiumPercentage, 2),
                TotalCount = total,
                PeriodLabel = month.HasValue ? $"{month.Value:D2}/{year}" : year.ToString()
            };
        }

        public List<UserGrowthPointResponse> GetUserGrowthByMonth(int year)
        {
            var currentYear = 2026; // ili DateTime.Now.Year
            var currentMonth = DateTime.Now.Month;
            var maxMonth = year == currentYear ? currentMonth : 12;

            var groupedData = _context.Users
                .Where(x => x.JoinDate.Year == year)
                .GroupBy(x => x.JoinDate.Month)
                .Select(x => new
                {
                    Month = x.Key,
                    Count = x.Count()
                })
                .ToList();

            var monthLabels = new Dictionary<int, string>
        {
            { 1, "Jan" },
            { 2, "Feb" },
            { 3, "Mar" },
            { 4, "Apr" },
            { 5, "May" },
            { 6, "Jun" },
            { 7, "Jul" },
            { 8, "Aug" },
            { 9, "Sep" },
            { 10, "Oct" },
            { 11, "Nov" },
            { 12, "Dec" }
        };

            var result = new List<UserGrowthPointResponse>();

            for (int month = 1; month <= maxMonth; month++)
            {
                var existing = groupedData.FirstOrDefault(x => x.Month == month);

                result.Add(new UserGrowthPointResponse
                {
                    Label = monthLabels[month],
                    Count = existing?.Count ?? 0
                });
            }

            return result;
        }
    }
}
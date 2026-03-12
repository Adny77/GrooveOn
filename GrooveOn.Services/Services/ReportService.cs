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
    var today = DateTime.Today;
    int currentYear = today.Year;
    int currentMonth = today.Month;

    int maxMonth;

    if (year < currentYear)
    {
        maxMonth = 12;
    }
    else if (year == currentYear)
    {
        maxMonth = currentMonth - 1;
    }
    else
    {
        return new List<UserGrowthPointResponse>();
    }

    if (maxMonth <= 0)
    {
        return new List<UserGrowthPointResponse>();
    }

    var result = _context.Users
        .Where(x => x.JoinDate.Year == year && x.JoinDate.Month <= maxMonth)
        .GroupBy(x => x.JoinDate.Month)
        .Select(g => new UserGrowthPointResponse
        {
            Month = g.Key,
            Count = g.Count()
        })
        .OrderBy(x => x.Month)
        .ToList();

    var monthLabels = new[]
    {
        "", "Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    };

    var completed = Enumerable.Range(1, maxMonth)
        .Select(month => new UserGrowthPointResponse
        {
            Month = month,
            Label = monthLabels[month],
            Count = result.FirstOrDefault(x => x.Month == month)?.Count ?? 0
        })
        .ToList();

    return completed;
}

        public List<IncomeByMonthResponse> GetIncomeByMonth(int year)
{
    var result = _context.Subscriptions
        .Where(x =>
            x.SubscriptionPlanId == 2 &&
            x.PaymentDate.HasValue &&
            x.PaymentDate.Value.Year == year)
        .GroupBy(x => x.PaymentDate!.Value.Month)
        .Select(g => new IncomeByMonthResponse
        {
            Month = g.Key,
            TotalIncome = g.Sum(x => x.PaymentAmount)
        })
        .OrderBy(x => x.Month)
        .ToList();

    var completed = Enumerable.Range(1, 12)
        .Select(month => new IncomeByMonthResponse
        {
            Month = month,
            TotalIncome = result.FirstOrDefault(x => x.Month == month)?.TotalIncome ?? 0
        })
        .ToList();

    return completed;
}
    }
}
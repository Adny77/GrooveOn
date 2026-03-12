using GrooveOn.Model.ResponseObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IReportService
    {
        SubscriptionAnalyticsResponse GetSubscriptionAnalytics(int year, int? month = null);
        List<UserGrowthPointResponse> GetUserGrowthByMonth(int year);
        List<IncomeByMonthResponse> GetIncomeByMonth(int year);
    }
}
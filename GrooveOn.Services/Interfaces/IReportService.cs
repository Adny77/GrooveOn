using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IReportService
    {
        SubscriptionAnalyticsResponse GetSubscriptionAnalytics(int year, int? month = null);
        List<UserGrowthPointResponse> GetUserGrowthByMonth(int year);
        SubscriptionAnalyticsResponse GetNewUsersSubscriptionAnalytics(int year, int? month);
        List<IncomeByMonthResponse> GetIncomeByMonth(int year);
        MobileHomeResponse GetMobileHome(int takeTracks = 4, int takeArtists = 8);
        MusicOverviewResponse GetMusicOverview(MusicOverviewRequest request);
    }
}
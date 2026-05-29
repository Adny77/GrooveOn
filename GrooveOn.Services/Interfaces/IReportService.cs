using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;

namespace GrooveOn.Services.Interfaces
{
    public interface IReportService
    {
        Task<SubscriptionAnalyticsResponse> GetSubscriptionAnalyticsAsync(int year, int? month = null);
        Task<List<UserGrowthPointResponse>> GetUserGrowthByMonthAsync(int year);
        Task<SubscriptionAnalyticsResponse> GetNewUsersSubscriptionAnalyticsAsync(int year, int? month);
        Task<List<IncomeByMonthResponse>> GetIncomeByMonthAsync(int year);
        Task<MobileHomeResponse> GetMobileHomeAsync(int takeTracks = 4, int takeArtists = 8);
        Task<MusicOverviewResponse> GetMusicOverviewAsync(MusicOverviewRequest request);
    }
}
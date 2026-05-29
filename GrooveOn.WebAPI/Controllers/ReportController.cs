using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GrooveOn.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService subscriptionService)
        {
            _reportService = subscriptionService;
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<SubscriptionAnalyticsResponse>> Get([FromQuery] int year, [FromQuery] int? month)
        {
            var result = await _reportService.GetSubscriptionAnalyticsAsync(year, month);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("user-growth-by-month")]
        public async Task<ActionResult<List<UserGrowthPointResponse>>> GetUserGrowthByMonth([FromQuery] int year)
        {
            var result = await _reportService.GetUserGrowthByMonthAsync(year);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("income-by-month")]
        public async Task<ActionResult<List<IncomeByMonthResponse>>> GetIncomeByMonth([FromQuery] int year)
        {
            return Ok(await _reportService.GetIncomeByMonthAsync(year));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("music-overview")]
        public async Task<ActionResult<MusicOverviewResponse>> GetMusicOverview([FromQuery] MusicOverviewRequest request)
        {
            var result = await _reportService.GetMusicOverviewAsync(request);
            return Ok(result);
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpGet("new-users-subscription-analytics")]
        public async Task<SubscriptionAnalyticsResponse> GetNewUsersSubscriptionAnalytics([FromQuery] int year, [FromQuery] int? month)
        {
            return await _reportService.GetNewUsersSubscriptionAnalyticsAsync(year, month);
        }

        [Authorize(Roles = Roles.UserAndAdmin)]
        [HttpGet("mobile-home")]
        public async Task<ActionResult<MobileHomeResponse>> GetMobileHome(
        [FromQuery] int takeTracks = 4,
        [FromQuery] int takeArtists = 8)
        {
            return Ok(await _reportService.GetMobileHomeAsync(takeTracks, takeArtists));
        }
    }
}

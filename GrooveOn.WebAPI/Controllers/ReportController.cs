using GrooveOn.Model.ResponseObjects;
using GrooveOn.Services.Interfaces;
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

        [HttpGet]
        public ActionResult<SubscriptionAnalyticsResponse> Get([FromQuery] int year, [FromQuery] int? month)
        {
            var result = _reportService.GetSubscriptionAnalytics(year, month);
            return Ok(result);
        }

        [HttpGet("user-growth-by-month")]
        public ActionResult<List<UserGrowthPointResponse>> GetUserGrowthByMonth(
        [FromQuery] int year)
        {
            var result = _reportService.GetUserGrowthByMonth(year);
            return Ok(result);
        }

        [HttpGet("income-by-month")]
        public ActionResult<List<IncomeByMonthResponse>> GetIncomeByMonth(int year)
        {
            return Ok(_reportService.GetIncomeByMonth(year));
        }
    }
}
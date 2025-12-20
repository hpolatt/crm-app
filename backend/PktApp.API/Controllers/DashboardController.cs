using Microsoft.AspNetCore.Mvc;
using PKT.Application.DTOs.Common;
using PKT.Application.Interfaces;

namespace PKT.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            var summary = await _dashboardService.GetSummaryAsync();
            return Ok(ApiResponse<object>.SuccessResponse(summary));
        }

        [HttpGet("reactor-analytics")]
        public async Task<IActionResult> GetReactorAnalytics()
        {
            var analytics = await _dashboardService.GetReactorAnalyticsAsync();
            return Ok(ApiResponse<object>.SuccessResponse(analytics));
        }

        [HttpGet("product-analytics")]
        public async Task<IActionResult> GetProductAnalytics([FromQuery] int topCount = 10)
        {
            var analytics = await _dashboardService.GetProductAnalyticsAsync(topCount);
            return Ok(ApiResponse<object>.SuccessResponse(analytics));
        }

        [HttpGet("delay-analytics")]
        public async Task<IActionResult> GetDelayAnalytics()
        {
            var analytics = await _dashboardService.GetDelayAnalyticsAsync();
            return Ok(ApiResponse<object>.SuccessResponse(analytics));
        }

        [HttpGet("daily-production")]
        public async Task<IActionResult> GetDailyProduction(
            [FromQuery] DateTime? startDate, 
            [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            
            var dailyData = await _dashboardService.GetDailyProductionAsync(start, end);
            return Ok(ApiResponse<object>.SuccessResponse(dailyData));
        }

        [HttpGet("status-distribution")]
        public async Task<IActionResult> GetStatusDistribution()
        {
            var distribution = await _dashboardService.GetStatusDistributionAsync();
            return Ok(ApiResponse<object>.SuccessResponse(distribution));
        }
    }
}

using PKT.Application.DTOs.Dashboard;

namespace PKT.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryDto> GetSummaryAsync();
        Task<List<ReactorAnalyticsDto>> GetReactorAnalyticsAsync();
        Task<List<ProductAnalyticsDto>> GetProductAnalyticsAsync(int topCount = 10);
        Task<List<DelayAnalyticsDto>> GetDelayAnalyticsAsync();
        Task<List<DailyProductionDto>> GetDailyProductionAsync(DateTime startDate, DateTime endDate);
        Task<List<StatusDistributionDto>> GetStatusDistributionAsync();
    }
}

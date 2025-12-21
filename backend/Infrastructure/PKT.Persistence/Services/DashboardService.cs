using Microsoft.EntityFrameworkCore;
using PKT.Application.DTOs.Dashboard;
using PKT.Application.Interfaces;
using PKT.Persistence.Data;

namespace PKT.Persistence.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var transactions = await _context.PktTransactions.ToListAsync();

            var summary = new DashboardSummaryDto
            {
                TotalProductionCount = transactions.Count,
                ActiveProductionCount = transactions.Count(t => t.Status == "Devam Ediyor"),
                CompletedProductionCount = transactions.Count(t => t.Status == "Tamamlandı"),
                AverageProductionDurationMinutes = transactions
                    .Where(t => t.ActualProductionDuration.HasValue)
                    .Select(t => t.ActualProductionDuration!.Value.TotalHours)
                    .DefaultIfEmpty(0)
                    .Average(),
                TotalDelayDurationHours = transactions
                    .Where(t => t.DelayDuration.HasValue)
                    .Sum(t => t.DelayDuration!.Value.TotalHours)
            };

            return summary;
        }

        public async Task<List<ReactorAnalyticsDto>> GetReactorAnalyticsAsync()
        {
            var analytics = await _context.PktTransactions
                .Include(t => t.Reactor)
                .GroupBy(t => new { t.ReactorId, t.Reactor.Name })
                .Select(g => new ReactorAnalyticsDto
                {
                    ReactorId = g.Key.ReactorId.ToString(),
                    ReactorName = g.Key.Name,
                    ProductionCount = g.Count(),
                    AverageProductionDurationMinutes = g
                        .Where(t => t.ActualProductionDuration.HasValue)
                        .Select(t => t.ActualProductionDuration!.Value.TotalHours)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderByDescending(r => r.ProductionCount)
                .ToListAsync();

            return analytics;
        }

        public async Task<List<ProductAnalyticsDto>> GetProductAnalyticsAsync(int topCount = 10)
        {
            var analytics = await _context.PktTransactions
                .Include(t => t.Product)
                .GroupBy(t => new 
                { 
                    t.ProductId, 
                    t.Product.ProductName,
                    t.Product.ProductCode,
                    t.Product.ProductionDurationMinutes 
                })
                .Select(g => new ProductAnalyticsDto
                {
                    ProductId = g.Key.ProductId.ToString(),
                    ProductName = g.Key.ProductName,
                    ProductCode = g.Key.ProductCode,
                    ProductionCount = g.Count(),
                    PlannedDurationMinutes = g.Key.ProductionDurationMinutes,
                    ActualAverageDurationMinutes = g
                        .Where(t => t.ActualProductionDuration.HasValue)
                        .Select(t => t.ActualProductionDuration!.Value.TotalHours)
                        .DefaultIfEmpty(0)
                        .Average()
                })
                .OrderByDescending(p => p.ProductionCount)
                .Take(topCount)
                .ToListAsync();

            // Calculate variance in memory after fetching
            foreach (var item in analytics)
            {
                item.VarianceMinutes = item.ActualAverageDurationMinutes - item.PlannedDurationMinutes;
            }

            return analytics;
        }

        public async Task<List<DelayAnalyticsDto>> GetDelayAnalyticsAsync()
        {
            var analytics = await _context.PktTransactions
                .Where(t => t.DelayReasonId.HasValue)
                .Include(t => t.DelayReason)
                .GroupBy(t => new 
                { 
                    t.DelayReasonId, 
                    DelayReasonName = t.DelayReason!.Name 
                })
                .Select(g => new DelayAnalyticsDto
                {
                    DelayReasonId = g.Key.DelayReasonId.ToString(),
                    DelayReasonName = g.Key.DelayReasonName,
                    ProductionCount = g.Count(),
                    TotalDelayDurationHours = g
                        .Where(t => t.DelayDuration.HasValue)
                        .Sum(t => t.DelayDuration!.Value.TotalHours)
                })
                .OrderByDescending(d => d.TotalDelayDurationHours)
                .ToListAsync();

            return analytics;
        }

        public async Task<List<DailyProductionDto>> GetDailyProductionAsync(DateTime startDate, DateTime endDate)
        {
            var dailyData = await _context.PktTransactions
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate)
                .GroupBy(t => t.CreatedAt.Date)
                .Select(g => new DailyProductionDto
                {
                    Date = g.Key,
                    ProductionCount = g.Count(),
                    TotalDelayDurationHours = g
                        .Where(t => t.DelayDuration.HasValue)
                        .Sum(t => t.DelayDuration!.Value.TotalHours)
                })
                .OrderBy(d => d.Date)
                .ToListAsync();

            return dailyData;
        }

        public async Task<List<StatusDistributionDto>> GetStatusDistributionAsync()
        {
            var distribution = await _context.PktTransactions
                .GroupBy(t => t.Status)
                .Select(g => new StatusDistributionDto
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(s => s.Count)
                .ToListAsync();

            return distribution;
        }
    }
}

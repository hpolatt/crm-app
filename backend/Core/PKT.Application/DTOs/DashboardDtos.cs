namespace PKT.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalProductionCount { get; set; }
        public int ActiveProductionCount { get; set; }
        public int CompletedProductionCount { get; set; }
        public double AverageProductionDurationMinutes { get; set; }
        public double TotalDelayDurationHours { get; set; }
    }

    public class ReactorAnalyticsDto
    {
        public string ReactorId { get; set; } = string.Empty;
        public string ReactorName { get; set; } = string.Empty;
        public int ProductionCount { get; set; }
        public double AverageProductionDurationMinutes { get; set; }
    }

    public class ProductAnalyticsDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int ProductionCount { get; set; }
        public int PlannedDurationMinutes { get; set; }
        public double ActualAverageDurationMinutes { get; set; }
        public double VarianceMinutes { get; set; }
    }

    public class DelayAnalyticsDto
    {
        public string? DelayReasonId { get; set; }
        public string DelayReasonName { get; set; } = string.Empty;
        public int ProductionCount { get; set; }
        public double TotalDelayDurationHours { get; set; }
    }

    public class DailyProductionDto
    {
        public DateTime Date { get; set; }
        public int ProductionCount { get; set; }
        public double TotalDelayDurationHours { get; set; }
    }

    public class StatusDistributionDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

namespace CRM.Application.DTOs.Reports;

public class SalesReportDto
{
    public List<MonthlySalesDto> MonthlySales { get; set; } = new();
    public List<SalesByStageDto> SalesByStage { get; set; } = new();
    public List<SalesByUserDto> SalesByUser { get; set; } = new();
    public decimal TotalRevenue { get; set; }
    public int TotalDeals { get; set; }
    public decimal AverageDealSize { get; set; }
}

public class MonthlySalesDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
    public int DealCount { get; set; }
}

public class SalesByStageDto
{
    public string Stage { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Value { get; set; }
    public double Percentage { get; set; }
}

public class SalesByUserDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public int DealCount { get; set; }
    public decimal Revenue { get; set; }
    public int WonCount { get; set; }
    public int LostCount { get; set; }
    public double WinRate { get; set; }
}

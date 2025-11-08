namespace CrmApp.Core.DTOs.Reports;

public class CustomerReportDto
{
    public List<CustomersBySourceDto> CustomersBySource { get; set; } = new();
    public List<CustomersByIndustryDto> CustomersByIndustry { get; set; } = new();
    public List<CustomerGrowthDto> CustomerGrowth { get; set; } = new();
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public int NewCustomersThisMonth { get; set; }
}

public class CustomersBySourceDto
{
    public string Source { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CustomersByIndustryDto
{
    public string Industry { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CustomerGrowthDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int NewCustomers { get; set; }
    public int TotalCustomers { get; set; }
}

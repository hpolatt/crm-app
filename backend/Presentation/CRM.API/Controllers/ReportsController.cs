using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.DTOs;
using CRM.Application.DTOs.Reports;
using CRM.Application.Interfaces;
using System.Globalization;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReportsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(IUnitOfWork unitOfWork, ILogger<ReportsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("sales")]
    public async Task<ActionResult<ApiResponse<SalesReportDto>>> GetSalesReport(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        try
        {
            startDate ??= DateTime.UtcNow.AddMonths(-12);
            endDate ??= DateTime.UtcNow;

            var opportunities = await _unitOfWork.Opportunities.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();

            var filteredOpportunities = opportunities
                .Where(o => !o.IsDeleted && 
                    o.CreatedAt >= startDate && 
                    o.CreatedAt <= endDate)
                .ToList();

            // Monthly Sales
            var monthlySales = filteredOpportunities
                .Where(o => o.Stage == "won" || o.Stage == "closed-won")
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new MonthlySalesDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Key.Month),
                    Revenue = g.Sum(o => o.Value),
                    DealCount = g.Count()
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            // Sales by Stage
            var totalOpps = filteredOpportunities.Count;
            var salesByStage = filteredOpportunities
                .GroupBy(o => o.Stage)
                .Select(g => new SalesByStageDto
                {
                    Stage = g.Key,
                    Count = g.Count(),
                    Value = g.Sum(o => o.Value),
                    Percentage = totalOpps > 0 ? Math.Round((double)g.Count() / totalOpps * 100, 2) : 0
                })
                .OrderByDescending(s => s.Value)
                .ToList();

            // Sales by User
            var salesByUser = filteredOpportunities
                .Where(o => o.AssignedUserId.HasValue)
                .GroupBy(o => o.AssignedUserId!.Value)
                .Select(g =>
                {
                    var user = users.FirstOrDefault(u => u.Id == g.Key);
                    var wonCount = g.Count(o => o.Stage == "won" || o.Stage == "closed-won");
                    var lostCount = g.Count(o => o.Stage == "lost" || o.Stage == "closed-lost");
                    var totalClosedCount = wonCount + lostCount;

                    return new SalesByUserDto
                    {
                        UserId = g.Key,
                        UserName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                        DealCount = g.Count(),
                        Revenue = g.Where(o => o.Stage == "won" || o.Stage == "closed-won").Sum(o => o.Value),
                        WonCount = wonCount,
                        LostCount = lostCount,
                        WinRate = totalClosedCount > 0 ? Math.Round((double)wonCount / totalClosedCount * 100, 2) : 0
                    };
                })
                .OrderByDescending(s => s.Revenue)
                .ToList();

            var wonOpps = filteredOpportunities.Where(o => o.Stage == "won" || o.Stage == "closed-won");
            var report = new SalesReportDto
            {
                MonthlySales = monthlySales,
                SalesByStage = salesByStage,
                SalesByUser = salesByUser,
                TotalRevenue = wonOpps.Sum(o => o.Value),
                TotalDeals = wonOpps.Count(),
                AverageDealSize = wonOpps.Any() ? wonOpps.Average(o => o.Value) : 0
            };

            return Ok(ApiResponse<SalesReportDto>.SuccessResponse(report, "Sales report retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sales report");
            return StatusCode(500, ApiResponse<SalesReportDto>.ErrorResponse(
                "An error occurred while retrieving sales report",
                new List<string> { ex.Message }
            ));
        }
    }

    [HttpGet("customers")]
    public async Task<ActionResult<ApiResponse<CustomerReportDto>>> GetCustomerReport()
    {
        try
        {
            var companies = await _unitOfWork.Companies.GetAllAsync();
            var activeCompanies = companies.Where(c => !c.IsDeleted).ToList();

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var newThisMonth = activeCompanies.Count(c => 
                c.CreatedAt.Month == currentMonth && 
                c.CreatedAt.Year == currentYear);

            // Customers by Source
            var totalCompanies = activeCompanies.Count;
            var customersBySource = activeCompanies
                .GroupBy(c => c.Source ?? "Unknown")
                .Select(g => new CustomersBySourceDto
                {
                    Source = g.Key,
                    Count = g.Count(),
                    Percentage = totalCompanies > 0 ? Math.Round((double)g.Count() / totalCompanies * 100, 2) : 0
                })
                .OrderByDescending(c => c.Count)
                .ToList();

            // Customers by Industry
            var customersByIndustry = activeCompanies
                .GroupBy(c => c.Industry ?? "Unknown")
                .Select(g => new CustomersByIndustryDto
                {
                    Industry = g.Key,
                    Count = g.Count(),
                    Percentage = totalCompanies > 0 ? Math.Round((double)g.Count() / totalCompanies * 100, 2) : 0
                })
                .OrderByDescending(c => c.Count)
                .ToList();

            // Customer Growth (last 12 months)
            var customerGrowth = new List<CustomerGrowthDto>();
            for (int i = 11; i >= 0; i--)
            {
                var date = DateTime.UtcNow.AddMonths(-i);
                var year = date.Year;
                var month = date.Month;

                var newCustomers = activeCompanies.Count(c => 
                    c.CreatedAt.Year == year && 
                    c.CreatedAt.Month == month);

                var totalCustomers = activeCompanies.Count(c => c.CreatedAt <= new DateTime(year, month, DateTime.DaysInMonth(year, month)));

                customerGrowth.Add(new CustomerGrowthDto
                {
                    Year = year,
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    NewCustomers = newCustomers,
                    TotalCustomers = totalCustomers
                });
            }

            var report = new CustomerReportDto
            {
                CustomersBySource = customersBySource,
                CustomersByIndustry = customersByIndustry,
                CustomerGrowth = customerGrowth,
                TotalCustomers = totalCompanies,
                ActiveCustomers = activeCompanies.Count(c => c.IsActive),
                NewCustomersThisMonth = newThisMonth
            };

            return Ok(ApiResponse<CustomerReportDto>.SuccessResponse(report, "Customer report retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving customer report");
            return StatusCode(500, ApiResponse<CustomerReportDto>.ErrorResponse(
                "An error occurred while retrieving customer report",
                new List<string> { ex.Message }
            ));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.DTOs;
using CRM.Application.DTOs.Dashboard;
using CRM.Application.Interfaces;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("summary")]
    public async Task<ActionResult<ApiResponse<DashboardSummaryDto>>> GetSummary()
    {
        try
        {
            var companies = await _unitOfWork.Companies.GetAllAsync();
            var contacts = await _unitOfWork.Contacts.GetAllAsync();
            var leads = await _unitOfWork.Leads.GetAllAsync();
            var opportunities = await _unitOfWork.Opportunities.GetAllAsync();
            var activities = await _unitOfWork.Activities.GetAllAsync();

            var activeCompanies = companies.Where(c => !c.IsDeleted && c.IsActive);
            var activeContacts = contacts.Where(c => !c.IsDeleted);
            var activeLeads = leads.Where(l => !l.IsDeleted && l.Status != "converted" && l.Status != "lost");
            var activeOpportunities = opportunities.Where(o => !o.IsDeleted);

            var wonOpportunities = activeOpportunities.Where(o => o.Stage == "won" || o.Stage == "closed-won");
            var lostOpportunities = activeOpportunities.Where(o => o.Stage == "lost" || o.Stage == "closed-lost");
            var openOpportunities = activeOpportunities.Where(o => 
                o.Stage != "won" && o.Stage != "closed-won" && 
                o.Stage != "lost" && o.Stage != "closed-lost");

            var today = DateTime.UtcNow.Date;
            var todayActivities = activities.Where(a => !a.IsDeleted && a.DueDate.HasValue && a.DueDate.Value.Date == today);
            var overdueActivities = activities.Where(a => !a.IsDeleted && a.DueDate.HasValue && 
                a.DueDate.Value.Date < today && a.Status != "completed");

            var totalWon = wonOpportunities.Count();
            var totalLost = lostOpportunities.Count();
            var winRate = totalWon + totalLost > 0 ? (double)totalWon / (totalWon + totalLost) * 100 : 0;

            var summary = new DashboardSummaryDto
            {
                TotalCompanies = activeCompanies.Count(),
                ActiveCompanies = activeCompanies.Count(c => c.IsActive),
                TotalContacts = activeContacts.Count(),
                TotalLeads = leads.Where(l => !l.IsDeleted).Count(),
                ActiveLeads = activeLeads.Count(),
                TotalOpportunities = activeOpportunities.Count(),
                OpenOpportunities = openOpportunities.Count(),
                WonOpportunities = wonOpportunities.Count(),
                LostOpportunities = lostOpportunities.Count(),
                TotalOpportunityValue = activeOpportunities.Sum(o => o.Value),
                WonOpportunityValue = wonOpportunities.Sum(o => o.Value),
                OpenOpportunityValue = openOpportunities.Sum(o => o.Value),
                TotalActivities = activities.Where(a => !a.IsDeleted).Count(),
                OverdueActivities = overdueActivities.Count(),
                TodayActivities = todayActivities.Count(),
                WinRate = Math.Round(winRate, 2),
                AverageDealSize = wonOpportunities.Any() ? wonOpportunities.Average(o => o.Value) : 0
            };

            return Ok(ApiResponse<DashboardSummaryDto>.SuccessResponse(summary, "Dashboard summary retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard summary");
            return StatusCode(500, ApiResponse<DashboardSummaryDto>.ErrorResponse(
                "An error occurred while retrieving dashboard summary",
                new List<string> { ex.Message }
            ));
        }
    }
}

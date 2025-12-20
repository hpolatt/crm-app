using Microsoft.AspNetCore.Mvc;
using PktApp.Core.DTOs.Common;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var totalTransactions = await _unitOfWork.PktTransactions.CountAsync();
            var reactors = await _unitOfWork.Reactors.GetAllAsync();
            var products = await _unitOfWork.Products.GetAllAsync();
            var delayReasons = await _unitOfWork.DelayReasons.GetAllAsync();

            var transactions = await _unitOfWork.PktTransactions.GetAllAsync();
            var totalDelayDurationHours = transactions
                .Where(t => t.DelayDuration.HasValue)
                .Sum(t => t.DelayDuration.Value.TotalHours);

            var summary = new
            {
                totalProductionCount = totalTransactions,
                activeProductionCount = reactors.Count(),
                completedProductionCount = products.Count(),
                totalDelayDurationHours = Math.Round(totalDelayDurationHours, 2)
            };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = summary
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard summary");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Dashboard özeti alınırken bir hata oluştu"
            });
        }
    }

    [HttpGet("reactor-analytics")]
    public async Task<IActionResult> GetReactorAnalytics()
    {
        try
        {
            var transactions = await _unitOfWork.PktTransactions.GetAllAsync();
            var reactors = await _unitOfWork.Reactors.GetAllAsync();

            var analytics = from reactor in reactors
                           join transaction in transactions on reactor.Id equals transaction.ReactorId into transGroup
                           select new
                           {
                               reactorName = reactor.Name,
                               productionCount = transGroup.Count()
                           };

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = analytics.ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reactor analytics");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Reaktör analitiği alınırken bir hata oluştu"
            });
        }
    }

    [HttpGet("status-distribution")]
    public async Task<IActionResult> GetStatusDistribution()
    {
        try
        {
            var transactions = await _unitOfWork.PktTransactions.GetAllAsync();

            var statusDistribution = transactions
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    status = g.Key.ToString(),
                    count = g.Count()
                })
                .ToList();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = statusDistribution
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting status distribution");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Durum dağılımı alınırken bir hata oluştu"
            });
        }
    }
}

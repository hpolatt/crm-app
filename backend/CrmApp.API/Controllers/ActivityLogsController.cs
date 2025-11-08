using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.Interfaces;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ActivityLogsController : BaseController
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<ActivityLogsController> _logger;

    public ActivityLogsController(
        IElasticsearchService elasticsearchService,
        ILogger<ActivityLogsController> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(
        [FromQuery] string? userId = null,
        [FromQuery] string? action = null,
        [FromQuery] string? path = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int? minStatusCode = null,
        [FromQuery] int? maxStatusCode = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var (logs, totalCount) = await _elasticsearchService.SearchActivityLogsAsync(
                userId: userId,
                action: action,
                path: path,
                startDate: startDate,
                endDate: endDate,
                minStatusCode: minStatusCode,
                maxStatusCode: maxStatusCode,
                pageNumber: pageNumber,
                pageSize: pageSize
            );

            var logsList = logs.Select(l => new
            {
                l.RequestId,
                l.Timestamp,
                l.Method,
                l.Path,
                l.StatusCode,
                l.DurationMs,
                l.UserId,
                l.UserEmail,
                l.IpAddress,
                l.UserAgent,
                l.ErrorMessage
            }).ToList();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Activity logs retrieved successfully from Elasticsearch",
                Data = new
                {
                    items = logsList,
                    totalCount,
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity logs");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving activity logs",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{requestId}")]
    public async Task<ActionResult<ApiResponse<object>>> GetByRequestId(string requestId)
    {
        try
        {
            // Özel bir metod kullanarak requestId'ye göre tek log getir
            var log = await _elasticsearchService.GetActivityLogByRequestIdAsync(requestId);

            if (log == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Activity log with RequestId '{requestId}' not found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Activity log retrieved successfully",
                Data = log
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity log with ID {RequestId}", requestId);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving the activity log",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

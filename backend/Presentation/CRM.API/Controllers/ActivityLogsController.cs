using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.DTOs;
using CRM.Application.DTOs.ActivityLogs;
using CRM.Application.Interfaces;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ActivityLogsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivityLogsController> _logger;

    public ActivityLogsController(IUnitOfWork unitOfWork, ILogger<ActivityLogsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(
        [FromQuery] Guid? userId = null,
        [FromQuery] string? entityType = null,
        [FromQuery] Guid? entityId = null,
        [FromQuery] string? action = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var logs = await _unitOfWork.ActivityLogs.GetAllAsync();
            var filteredLogs = logs.Where(l => !l.IsDeleted);

            if (userId.HasValue)
                filteredLogs = filteredLogs.Where(l => l.UserId == userId.Value);

            if (!string.IsNullOrEmpty(entityType))
                filteredLogs = filteredLogs.Where(l => l.EntityType.ToLower() == entityType.ToLower());

            if (entityId.HasValue)
                filteredLogs = filteredLogs.Where(l => l.EntityId == entityId.Value);

            if (!string.IsNullOrEmpty(action))
                filteredLogs = filteredLogs.Where(l => l.Action.ToLower() == action.ToLower());

            if (startDate.HasValue)
                filteredLogs = filteredLogs.Where(l => l.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                filteredLogs = filteredLogs.Where(l => l.CreatedAt <= endDate.Value);

            var totalCount = filteredLogs.Count();
            var logsList = filteredLogs
                .OrderByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new ActivityLogDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    UserName = l.UserName,
                    Action = l.Action,
                    EntityType = l.EntityType,
                    EntityId = l.EntityId,
                    EntityName = l.EntityName,
                    Details = l.Description,
                    IpAddress = l.IpAddress,
                    UserAgent = l.UserAgent,
                    CreatedAt = l.CreatedAt
                })
                .ToList();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Activity logs retrieved successfully",
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

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ActivityLogDto>>> GetById(Guid id)
    {
        try
        {
            var logs = await _unitOfWork.ActivityLogs.GetAllAsync();
            var log = logs.FirstOrDefault(l => l.Id == id && !l.IsDeleted);

            if (log == null)
            {
                return NotFound(ApiResponse<ActivityLogDto>.ErrorResponse(
                    "Activity log not found",
                    new List<string> { "Activity log not found" }
                ));
            }

            var logDto = new ActivityLogDto
            {
                Id = log.Id,
                UserId = log.UserId,
                UserName = log.UserName,
                Action = log.Action,
                EntityType = log.EntityType,
                EntityId = log.EntityId,
                EntityName = log.EntityName,
                Details = log.Description,
                IpAddress = log.IpAddress,
                UserAgent = log.UserAgent,
                CreatedAt = log.CreatedAt
            };

            return Ok(ApiResponse<ActivityLogDto>.SuccessResponse(
                logDto, 
                "Activity log retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity log");
            return StatusCode(500, ApiResponse<ActivityLogDto>.ErrorResponse(
                "An error occurred while retrieving activity log",
                new List<string> { ex.Message }
            ));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Activities;
using CrmApp.Core.DTOs.Common;
using CrmApp.Core.Interfaces;
using CrmApp.Domain.Entities;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ActivitiesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ActivitiesController> _logger;

    public ActivitiesController(IUnitOfWork unitOfWork, ILogger<ActivitiesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ActivityDto>>>> GetAll(
        [FromQuery] ActivityFilterQuery filter,
        [FromQuery] PaginationQuery pagination)
    {
        try
        {
            var allActivities = await _unitOfWork.Activities.GetAllAsync();
            var filteredActivities = allActivities.Where(a => !a.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(filter.Type))
                filteredActivities = filteredActivities.Where(a => a.Type == filter.Type);

            if (!string.IsNullOrEmpty(filter.Status))
                filteredActivities = filteredActivities.Where(a => a.Status == filter.Status);

            if (filter.AssignedUserId.HasValue)
                filteredActivities = filteredActivities.Where(a => a.AssignedUserId == filter.AssignedUserId.Value);

            if (filter.CompanyId.HasValue)
                filteredActivities = filteredActivities.Where(a => a.CompanyId == filter.CompanyId.Value);

            if (filter.ContactId.HasValue)
                filteredActivities = filteredActivities.Where(a => a.ContactId == filter.ContactId.Value);

            if (filter.LeadId.HasValue)
                filteredActivities = filteredActivities.Where(a => a.LeadId == filter.LeadId.Value);

            if (filter.OpportunityId.HasValue)
                filteredActivities = filteredActivities.Where(a => a.OpportunityId == filter.OpportunityId.Value);

            if (filter.IsActive.HasValue)
                filteredActivities = filteredActivities.Where(a => a.IsActive == filter.IsActive.Value);

            if (!string.IsNullOrEmpty(filter.Priority))
                filteredActivities = filteredActivities.Where(a => a.Priority == filter.Priority);

            if (filter.DueDateFrom.HasValue)
                filteredActivities = filteredActivities.Where(a => a.DueDate >= filter.DueDateFrom.Value);

            if (filter.DueDateTo.HasValue)
                filteredActivities = filteredActivities.Where(a => a.DueDate <= filter.DueDateTo.Value);

            var totalCount = filteredActivities.Count();
            var activities = filteredActivities
                .OrderByDescending(a => a.DueDate ?? a.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    Subject = a.Subject,
                    Description = a.Description,
                    Status = a.Status,
                    Priority = a.Priority,
                    DueDate = a.DueDate,
                    CompletedDate = a.CompletedDate,
                    CompanyId = a.CompanyId,
                    CompanyName = a.Company?.Name,
                    ContactId = a.ContactId,
                    ContactName = a.Contact != null ? $"{a.Contact.FirstName} {a.Contact.LastName}" : null,
                    LeadId = a.LeadId,
                    OpportunityId = a.OpportunityId,
                    OpportunityTitle = a.Opportunity?.Title,
                    AssignedUserId = a.AssignedUserId,
                    AssignedUserIdName = a.AssignedUser != null ? $"{a.AssignedUser.FirstName} {a.AssignedUser.LastName}" : null,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToList();

            var pagedResult = new PagedResult<ActivityDto>
            {
                Items = activities,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(new ApiResponse<PagedResult<ActivityDto>>
            {
                Success = true,
                Message = "Activities retrieved successfully",
                Data = pagedResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activities");
            return StatusCode(500, new ApiResponse<PagedResult<ActivityDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving activities",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ActivityDto>>> GetById(Guid id)
    {
        try
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);
            
            if (activity == null || activity.IsDeleted)
            {
                return NotFound(new ApiResponse<ActivityDto>
                {
                    Success = false,
                    Message = "Activity not found"
                });
            }

            var activityDto = new ActivityDto
            {
                Id = activity.Id,
                Type = activity.Type,
                Subject = activity.Subject,
                Description = activity.Description,
                Status = activity.Status,
                Priority = activity.Priority,
                DueDate = activity.DueDate,
                CompletedDate = activity.CompletedDate,
                CompanyId = activity.CompanyId,
                CompanyName = activity.Company?.Name,
                ContactId = activity.ContactId,
                ContactName = activity.Contact != null ? $"{activity.Contact.FirstName} {activity.Contact.LastName}" : null,
                LeadId = activity.LeadId,
                OpportunityId = activity.OpportunityId,
                OpportunityTitle = activity.Opportunity?.Title,
                AssignedUserId = activity.AssignedUserId,
                AssignedUserIdName = activity.AssignedUser != null ? $"{activity.AssignedUser.FirstName} {activity.AssignedUser.LastName}" : null,
                IsActive = activity.IsActive,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt
            };

            return Ok(new ApiResponse<ActivityDto>
            {
                Success = true,
                Message = "Activity retrieved successfully",
                Data = activityDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity {Id}", id);
            return StatusCode(500, new ApiResponse<ActivityDto>
            {
                Success = false,
                Message = "An error occurred while retrieving activity",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<ActivityDto>>>> GetByUser(Guid userId)
    {
        try
        {
            var allActivities = await _unitOfWork.Activities.GetAllAsync();
            var activities = allActivities
                .Where(a => !a.IsDeleted && a.AssignedUserId == userId)
                .OrderByDescending(a => a.DueDate ?? a.CreatedAt)
                .Select(a => new ActivityDto
                {
                    Id = a.Id,
                    Type = a.Type,
                    Subject = a.Subject,
                    Description = a.Description,
                    Status = a.Status,
                    Priority = a.Priority,
                    DueDate = a.DueDate,
                    CompletedDate = a.CompletedDate,
                    CompanyId = a.CompanyId,
                    CompanyName = a.Company?.Name,
                    ContactId = a.ContactId,
                    ContactName = a.Contact != null ? $"{a.Contact.FirstName} {a.Contact.LastName}" : null,
                    LeadId = a.LeadId,
                    OpportunityId = a.OpportunityId,
                    OpportunityTitle = a.Opportunity?.Title,
                    AssignedUserId = a.AssignedUserId,
                    AssignedUserIdName = a.AssignedUser != null ? $"{a.AssignedUser.FirstName} {a.AssignedUser.LastName}" : null,
                    IsActive = a.IsActive,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                })
                .ToList();

            return Ok(new ApiResponse<List<ActivityDto>>
            {
                Success = true,
                Message = "User activities retrieved successfully",
                Data = activities
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activities for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<List<ActivityDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving user activities",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ActivityDto>>> Create([FromBody] CreateActivityDto dto)
    {
        try
        {
            var activity = new Activity
            {
                Type = dto.Type,
                Subject = dto.Subject,
                Description = dto.Description,
                Status = dto.Status,
                Priority = dto.Priority,
                DueDate = dto.DueDate,
                CompanyId = dto.CompanyId,
                ContactId = dto.ContactId,
                LeadId = dto.LeadId,
                OpportunityId = dto.OpportunityId,
                AssignedUserId = dto.AssignedUserId,
                IsActive = dto.IsActive
            };

            await _unitOfWork.Activities.AddAsync(activity);
            await _unitOfWork.SaveChangesAsync();

            var activityDto = new ActivityDto
            {
                Id = activity.Id,
                Type = activity.Type,
                Subject = activity.Subject,
                Description = activity.Description,
                Status = activity.Status,
                Priority = activity.Priority,
                DueDate = activity.DueDate,
                CompletedDate = activity.CompletedDate,
                CompanyId = activity.CompanyId,
                ContactId = activity.ContactId,
                LeadId = activity.LeadId,
                OpportunityId = activity.OpportunityId,
                AssignedUserId = activity.AssignedUserId,
                IsActive = activity.IsActive,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = activity.Id }, new ApiResponse<ActivityDto>
            {
                Success = true,
                Message = "Activity created successfully",
                Data = activityDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating activity");
            return StatusCode(500, new ApiResponse<ActivityDto>
            {
                Success = false,
                Message = "An error occurred while creating activity",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ActivityDto>>> Update(Guid id, [FromBody] UpdateActivityDto dto)
    {
        try
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);
            
            if (activity == null || activity.IsDeleted)
            {
                return NotFound(new ApiResponse<ActivityDto>
                {
                    Success = false,
                    Message = "Activity not found"
                });
            }

            activity.Type = dto.Type;
            activity.Subject = dto.Subject;
            activity.Description = dto.Description;
            activity.Status = dto.Status;
            activity.Priority = dto.Priority;
            activity.DueDate = dto.DueDate;
            activity.CompletedDate = dto.CompletedDate;
            activity.CompanyId = dto.CompanyId;
            activity.ContactId = dto.ContactId;
            activity.LeadId = dto.LeadId;
            activity.OpportunityId = dto.OpportunityId;
            activity.AssignedUserId = dto.AssignedUserId;
            activity.IsActive = dto.IsActive;

            _unitOfWork.Activities.Update(activity);
            await _unitOfWork.SaveChangesAsync();

            var activityDto = new ActivityDto
            {
                Id = activity.Id,
                Type = activity.Type,
                Subject = activity.Subject,
                Description = activity.Description,
                Status = activity.Status,
                Priority = activity.Priority,
                DueDate = activity.DueDate,
                CompletedDate = activity.CompletedDate,
                CompanyId = activity.CompanyId,
                ContactId = activity.ContactId,
                LeadId = activity.LeadId,
                OpportunityId = activity.OpportunityId,
                AssignedUserId = activity.AssignedUserId,
                IsActive = activity.IsActive,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt
            };

            return Ok(new ApiResponse<ActivityDto>
            {
                Success = true,
                Message = "Activity updated successfully",
                Data = activityDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating activity {Id}", id);
            return StatusCode(500, new ApiResponse<ActivityDto>
            {
                Success = false,
                Message = "An error occurred while updating activity",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(id);
            
            if (activity == null || activity.IsDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Activity not found"
                });
            }

            activity.IsDeleted = true;
            _unitOfWork.Activities.Update(activity);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Activity deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting activity {Id}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting activity",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

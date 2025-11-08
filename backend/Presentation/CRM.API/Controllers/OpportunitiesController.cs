using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.DTOs;
using CRM.Application.DTOs.Opportunities;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;

namespace CRM.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OpportunitiesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OpportunitiesController> _logger;

    public OpportunitiesController(IUnitOfWork unitOfWork, ILogger<OpportunitiesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll(
        [FromQuery] string? stage = null,
        [FromQuery] Guid? AssignedUserId = null,
        [FromQuery] Guid? companyId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var allOpportunities = await _unitOfWork.Opportunities.GetAllAsync();
            var filteredOpportunities = allOpportunities.Where(o => !o.IsDeleted);

            if (!string.IsNullOrEmpty(stage))
            {
                filteredOpportunities = filteredOpportunities.Where(o => o.Stage == stage);
            }

            if (AssignedUserId.HasValue)
            {
                filteredOpportunities = filteredOpportunities.Where(o => o.AssignedUserId == AssignedUserId.Value);
            }

            if (companyId.HasValue)
            {
                filteredOpportunities = filteredOpportunities.Where(o => o.CompanyId == companyId.Value);
            }

            if (isActive.HasValue)
            {
                filteredOpportunities = filteredOpportunities.Where(o => o.IsActive == isActive.Value);
            }

            var totalCount = filteredOpportunities.Count();
            var opportunities = filteredOpportunities
                .OrderByDescending(o => o.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OpportunityDto
                {
                    Id = o.Id,
                    LeadId = o.LeadId,
                    CompanyId = o.CompanyId,
                    CompanyName = o.Company?.Name,
                    ContactId = o.ContactId,
                    ContactName = o.Contact != null ? $"{o.Contact.FirstName} {o.Contact.LastName}" : null,
                    Title = o.Title,
                    Description = o.Description,
                    Stage = o.Stage,
                    Value = o.Value,
                    Probability = o.Probability,
                    ExpectedCloseDate = o.ExpectedCloseDate,
                    ActualCloseDate = o.ActualCloseDate,
                    AssignedUserId = o.AssignedUserId,
                    AssignedUserIdName = o.AssignedUser != null ? $"{o.AssignedUser.FirstName} {o.AssignedUser.LastName}" : null,
                    Notes = o.Notes,
                    IsActive = o.IsActive,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt
                })
                .ToList();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Opportunities retrieved successfully",
                Data = new
                {
                    items = opportunities,
                    totalCount,
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunities");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving opportunities",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OpportunityDto>>> GetById(Guid id)
    {
        try
        {
            var opportunity = await _unitOfWork.Opportunities.GetByIdAsync(id);
            
            if (opportunity == null || opportunity.IsDeleted)
            {
                return NotFound(new ApiResponse<OpportunityDto>
                {
                    Success = false,
                    Message = "Opportunity not found"
                });
            }

            var opportunityDto = new OpportunityDto
            {
                Id = opportunity.Id,
                LeadId = opportunity.LeadId,
                CompanyId = opportunity.CompanyId,
                CompanyName = opportunity.Company?.Name,
                ContactId = opportunity.ContactId,
                ContactName = opportunity.Contact != null ? $"{opportunity.Contact.FirstName} {opportunity.Contact.LastName}" : null,
                Title = opportunity.Title,
                Description = opportunity.Description,
                Stage = opportunity.Stage,
                Value = opportunity.Value,
                Probability = opportunity.Probability,
                ExpectedCloseDate = opportunity.ExpectedCloseDate,
                ActualCloseDate = opportunity.ActualCloseDate,
                AssignedUserId = opportunity.AssignedUserId,
                AssignedUserIdName = opportunity.AssignedUser != null ? $"{opportunity.AssignedUser.FirstName} {opportunity.AssignedUser.LastName}" : null,
                Notes = opportunity.Notes,
                IsActive = opportunity.IsActive,
                CreatedAt = opportunity.CreatedAt,
                UpdatedAt = opportunity.UpdatedAt
            };

            return Ok(new ApiResponse<OpportunityDto>
            {
                Success = true,
                Message = "Opportunity retrieved successfully",
                Data = opportunityDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunity {Id}", id);
            return StatusCode(500, new ApiResponse<OpportunityDto>
            {
                Success = false,
                Message = "An error occurred while retrieving opportunity",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponse<OpportunityStatsDto>>> GetStats([FromQuery] Guid? AssignedUserId = null)
    {
        try
        {
            var allOpportunities = await _unitOfWork.Opportunities.GetAllAsync();
            var opportunities = allOpportunities.Where(o => !o.IsDeleted && o.IsActive);

            if (AssignedUserId.HasValue)
            {
                opportunities = opportunities.Where(o => o.AssignedUserId == AssignedUserId.Value);
            }

            var stats = new OpportunityStatsDto
            {
                TotalCount = opportunities.Count(),
                TotalValue = opportunities.Sum(o => o.Value),
                WonCount = opportunities.Count(o => o.Stage == "won"),
                WonValue = opportunities.Where(o => o.Stage == "won").Sum(o => o.Value),
                LostCount = opportunities.Count(o => o.Stage == "lost"),
                LostValue = opportunities.Where(o => o.Stage == "lost").Sum(o => o.Value),
                OpenCount = opportunities.Count(o => o.Stage != "won" && o.Stage != "lost"),
                OpenValue = opportunities.Where(o => o.Stage != "won" && o.Stage != "lost").Sum(o => o.Value),
                ByStage = opportunities.GroupBy(o => o.Stage).ToDictionary(g => g.Key, g => g.Count()),
                ValueByStage = opportunities.GroupBy(o => o.Stage).ToDictionary(g => g.Key, g => g.Sum(o => o.Value))
            };

            var totalDecided = stats.WonCount + stats.LostCount;
            stats.WinRate = totalDecided > 0 ? (double)stats.WonCount / totalDecided * 100 : 0;

            return Ok(new ApiResponse<OpportunityStatsDto>
            {
                Success = true,
                Message = "Opportunity stats retrieved successfully",
                Data = stats
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving opportunity stats");
            return StatusCode(500, new ApiResponse<OpportunityStatsDto>
            {
                Success = false,
                Message = "An error occurred while retrieving opportunity stats",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OpportunityDto>>> Create([FromBody] CreateOpportunityDto dto)
    {
        try
        {
            var opportunity = new Opportunity
            {
                LeadId = dto.LeadId,
                CompanyId = dto.CompanyId,
                ContactId = dto.ContactId,
                Title = dto.Title,
                Description = dto.Description,
                Stage = dto.Stage,
                Value = dto.Value,
                Probability = dto.Probability,
                ExpectedCloseDate = dto.ExpectedCloseDate,
                AssignedUserId = dto.AssignedUserId,
                Notes = dto.Notes,
                IsActive = dto.IsActive
            };

            await _unitOfWork.Opportunities.AddAsync(opportunity);
            await _unitOfWork.SaveChangesAsync();

            var opportunityDto = new OpportunityDto
            {
                Id = opportunity.Id,
                LeadId = opportunity.LeadId,
                CompanyId = opportunity.CompanyId,
                ContactId = opportunity.ContactId,
                Title = opportunity.Title,
                Description = opportunity.Description,
                Stage = opportunity.Stage,
                Value = opportunity.Value,
                Probability = opportunity.Probability,
                ExpectedCloseDate = opportunity.ExpectedCloseDate,
                ActualCloseDate = opportunity.ActualCloseDate,
                AssignedUserId = opportunity.AssignedUserId,
                Notes = opportunity.Notes,
                IsActive = opportunity.IsActive,
                CreatedAt = opportunity.CreatedAt,
                UpdatedAt = opportunity.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = opportunity.Id }, new ApiResponse<OpportunityDto>
            {
                Success = true,
                Message = "Opportunity created successfully",
                Data = opportunityDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating opportunity");
            return StatusCode(500, new ApiResponse<OpportunityDto>
            {
                Success = false,
                Message = "An error occurred while creating opportunity",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<OpportunityDto>>> Update(Guid id, [FromBody] UpdateOpportunityDto dto)
    {
        try
        {
            var opportunity = await _unitOfWork.Opportunities.GetByIdAsync(id);
            
            if (opportunity == null || opportunity.IsDeleted)
            {
                return NotFound(new ApiResponse<OpportunityDto>
                {
                    Success = false,
                    Message = "Opportunity not found"
                });
            }

            opportunity.LeadId = dto.LeadId;
            opportunity.CompanyId = dto.CompanyId;
            opportunity.ContactId = dto.ContactId;
            opportunity.Title = dto.Title;
            opportunity.Description = dto.Description;
            opportunity.Stage = dto.Stage;
            opportunity.Value = dto.Value;
            opportunity.Probability = dto.Probability;
            opportunity.ExpectedCloseDate = dto.ExpectedCloseDate;
            opportunity.ActualCloseDate = dto.ActualCloseDate;
            opportunity.AssignedUserId = dto.AssignedUserId;
            opportunity.Notes = dto.Notes;
            opportunity.IsActive = dto.IsActive;

            _unitOfWork.Opportunities.Update(opportunity);
            await _unitOfWork.SaveChangesAsync();

            var opportunityDto = new OpportunityDto
            {
                Id = opportunity.Id,
                LeadId = opportunity.LeadId,
                CompanyId = opportunity.CompanyId,
                ContactId = opportunity.ContactId,
                Title = opportunity.Title,
                Description = opportunity.Description,
                Stage = opportunity.Stage,
                Value = opportunity.Value,
                Probability = opportunity.Probability,
                ExpectedCloseDate = opportunity.ExpectedCloseDate,
                ActualCloseDate = opportunity.ActualCloseDate,
                AssignedUserId = opportunity.AssignedUserId,
                Notes = opportunity.Notes,
                IsActive = opportunity.IsActive,
                CreatedAt = opportunity.CreatedAt,
                UpdatedAt = opportunity.UpdatedAt
            };

            return Ok(new ApiResponse<OpportunityDto>
            {
                Success = true,
                Message = "Opportunity updated successfully",
                Data = opportunityDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating opportunity {Id}", id);
            return StatusCode(500, new ApiResponse<OpportunityDto>
            {
                Success = false,
                Message = "An error occurred while updating opportunity",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            var opportunity = await _unitOfWork.Opportunities.GetByIdAsync(id);
            
            if (opportunity == null || opportunity.IsDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Opportunity not found"
                });
            }

            opportunity.IsDeleted = true;
            _unitOfWork.Opportunities.Update(opportunity);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Opportunity deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting opportunity {Id}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting opportunity",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

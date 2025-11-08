using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Leads;
using CrmApp.Core.DTOs.Common;
using CrmApp.Core.Interfaces;
using CrmApp.Domain.Entities;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LeadsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LeadsController> _logger;

    public LeadsController(IUnitOfWork unitOfWork, ILogger<LeadsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LeadDto>>>> GetAll(
        [FromQuery] LeadFilterQuery filter,
        [FromQuery] PaginationQuery pagination)
    {
        try
        {
            var allLeads = await _unitOfWork.Leads.GetAllAsync();
            var filteredLeads = allLeads.Where(l => !l.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                filteredLeads = filteredLeads.Where(l =>
                    (l.Title != null && l.Title.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (l.Description != null && l.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(filter.Status))
                filteredLeads = filteredLeads.Where(l => l.Status == filter.Status);

            if (!string.IsNullOrEmpty(filter.Source))
                filteredLeads = filteredLeads.Where(l => l.Source == filter.Source);

            if (filter.AssignedUserId.HasValue)
                filteredLeads = filteredLeads.Where(l => l.AssignedUserId == filter.AssignedUserId.Value);

            if (filter.IsActive.HasValue)
                filteredLeads = filteredLeads.Where(l => l.IsActive == filter.IsActive.Value);

            if (filter.CreatedFrom.HasValue)
                filteredLeads = filteredLeads.Where(l => l.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                filteredLeads = filteredLeads.Where(l => l.CreatedAt <= filter.CreatedTo.Value);

            var totalCount = filteredLeads.Count();
            var leads = filteredLeads
                .OrderByDescending(l => l.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .Select(l => new LeadDto
                {
                    Id = l.Id,
                    CompanyId = l.CompanyId,
                    ContactId = l.ContactId,
                    Title = l.Title,
                    Description = l.Description,
                    Source = l.Source,
                    Status = l.Status,
                    Value = l.Value,
                    Probability = l.Probability,
                    ExpectedCloseDate = l.ExpectedCloseDate,
                    AssignedUserId = l.AssignedUserId,
                    Notes = l.Notes,
                    IsActive = l.IsActive,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .ToList();

            var pagedResult = new PagedResult<LeadDto>
            {
                Items = leads,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(new ApiResponse<PagedResult<LeadDto>>
            {
                Success = true,
                Message = "Leads retrieved successfully",
                Data = pagedResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving leads");
            return StatusCode(500, new ApiResponse<PagedResult<LeadDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving leads",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<LeadDto>>> GetById(Guid id)
    {
        try
        {
            var lead = await _unitOfWork.Leads.GetByIdAsync(id);
            
            if (lead == null || lead.IsDeleted)
            {
                return NotFound(new ApiResponse<LeadDto>
                {
                    Success = false,
                    Message = "Lead not found"
                });
            }

            var leadDto = new LeadDto
            {
                Id = lead.Id,
                CompanyId = lead.CompanyId,
                ContactId = lead.ContactId,
                Title = lead.Title,
                Description = lead.Description,
                Source = lead.Source,
                Status = lead.Status,
                Value = lead.Value,
                Probability = lead.Probability,
                ExpectedCloseDate = lead.ExpectedCloseDate,
                AssignedUserId = lead.AssignedUserId,
                Notes = lead.Notes,
                IsActive = lead.IsActive,
                CreatedAt = lead.CreatedAt,
                UpdatedAt = lead.UpdatedAt
            };

            return Ok(new ApiResponse<LeadDto>
            {
                Success = true,
                Message = "Lead retrieved successfully",
                Data = leadDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving lead {LeadId}", id);
            return StatusCode(500, new ApiResponse<LeadDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the lead",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<LeadDto>>> Create([FromBody] CreateLeadDto createDto)
    {
        try
        {
            var lead = new Lead
            {
                CompanyId = createDto.CompanyId,
                ContactId = createDto.ContactId,
                Title = createDto.Title,
                Description = createDto.Description,
                Source = createDto.Source,
                Status = createDto.Status ?? "new",
                Value = createDto.Value,
                Probability = createDto.Probability,
                ExpectedCloseDate = createDto.ExpectedCloseDate,
                AssignedUserId = createDto.AssignedUserId,
                Notes = createDto.Notes,
                IsActive = true
            };

            await _unitOfWork.Leads.AddAsync(lead);
            await _unitOfWork.SaveChangesAsync();

            var leadDto = new LeadDto
            {
                Id = lead.Id,
                CompanyId = lead.CompanyId,
                ContactId = lead.ContactId,
                Title = lead.Title,
                Description = lead.Description,
                Source = lead.Source,
                Status = lead.Status,
                Value = lead.Value,
                Probability = lead.Probability,
                ExpectedCloseDate = lead.ExpectedCloseDate,
                AssignedUserId = lead.AssignedUserId,
                Notes = lead.Notes,
                IsActive = lead.IsActive,
                CreatedAt = lead.CreatedAt,
                UpdatedAt = lead.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = lead.Id }, new ApiResponse<LeadDto>
            {
                Success = true,
                Message = "Lead created successfully",
                Data = leadDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lead");
            return StatusCode(500, new ApiResponse<LeadDto>
            {
                Success = false,
                Message = "An error occurred while creating the lead",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<LeadDto>>> Update(Guid id, [FromBody] UpdateLeadDto updateDto)
    {
        try
        {
            var lead = await _unitOfWork.Leads.GetByIdAsync(id);
            
            if (lead == null || lead.IsDeleted)
            {
                return NotFound(new ApiResponse<LeadDto>
                {
                    Success = false,
                    Message = "Lead not found"
                });
            }

            if (updateDto.CompanyId.HasValue)
                lead.CompanyId = updateDto.CompanyId;
            if (updateDto.ContactId.HasValue)
                lead.ContactId = updateDto.ContactId;
            if (!string.IsNullOrEmpty(updateDto.Title))
                lead.Title = updateDto.Title;
            if (updateDto.Description != null)
                lead.Description = updateDto.Description;
            if (updateDto.Source != null)
                lead.Source = updateDto.Source;
            if (updateDto.Status != null)
                lead.Status = updateDto.Status;
            if (updateDto.Value.HasValue)
                lead.Value = updateDto.Value;
            if (updateDto.Probability.HasValue)
                lead.Probability = updateDto.Probability;
            if (updateDto.ExpectedCloseDate.HasValue)
                lead.ExpectedCloseDate = updateDto.ExpectedCloseDate;
            if (updateDto.AssignedUserId.HasValue)
                lead.AssignedUserId = updateDto.AssignedUserId;
            if (updateDto.Notes != null)
                lead.Notes = updateDto.Notes;
            if (updateDto.IsActive.HasValue)
                lead.IsActive = updateDto.IsActive.Value;

            _unitOfWork.Leads.Update(lead);
            await _unitOfWork.SaveChangesAsync();

            var leadDto = new LeadDto
            {
                Id = lead.Id,
                CompanyId = lead.CompanyId,
                ContactId = lead.ContactId,
                Title = lead.Title,
                Description = lead.Description,
                Source = lead.Source,
                Status = lead.Status,
                Value = lead.Value,
                Probability = lead.Probability,
                ExpectedCloseDate = lead.ExpectedCloseDate,
                AssignedUserId = lead.AssignedUserId,
                Notes = lead.Notes,
                IsActive = lead.IsActive,
                CreatedAt = lead.CreatedAt,
                UpdatedAt = lead.UpdatedAt
            };

            return Ok(new ApiResponse<LeadDto>
            {
                Success = true,
                Message = "Lead updated successfully",
                Data = leadDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lead {LeadId}", id);
            return StatusCode(500, new ApiResponse<LeadDto>
            {
                Success = false,
                Message = "An error occurred while updating the lead",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            var lead = await _unitOfWork.Leads.GetByIdAsync(id);
            
            if (lead == null || lead.IsDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Lead not found"
                });
            }

            lead.IsDeleted = true;
            _unitOfWork.Leads.Update(lead);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Lead deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lead {LeadId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the lead",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

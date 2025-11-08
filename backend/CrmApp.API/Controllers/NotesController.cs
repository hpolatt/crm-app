using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Notes;
using CrmApp.Core.DTOs.Common;
using CrmApp.Core.Interfaces;
using CrmApp.Domain.Entities;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<NotesController> _logger;

    public NotesController(IUnitOfWork unitOfWork, ILogger<NotesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<NoteDto>>>> GetAll(
        [FromQuery] NoteFilterQuery filter,
        [FromQuery] PaginationQuery pagination)
    {
        try
        {
            var allNotes = await _unitOfWork.Notes.GetAllAsync();
            var filteredNotes = allNotes.Where(n => !n.IsDeleted && n.IsActive);

            // Apply filters
            if (filter.CompanyId.HasValue)
                filteredNotes = filteredNotes.Where(n => n.CompanyId == filter.CompanyId.Value);

            if (filter.ContactId.HasValue)
                filteredNotes = filteredNotes.Where(n => n.ContactId == filter.ContactId.Value);

            if (filter.LeadId.HasValue)
                filteredNotes = filteredNotes.Where(n => n.LeadId == filter.LeadId.Value);

            if (filter.OpportunityId.HasValue)
                filteredNotes = filteredNotes.Where(n => n.OpportunityId == filter.OpportunityId.Value);

            if (filter.CreatedBy.HasValue)
                filteredNotes = filteredNotes.Where(n => n.CreatedBy == filter.CreatedBy.Value);

            if (filter.IsActive.HasValue)
                filteredNotes = filteredNotes.Where(n => n.IsActive == filter.IsActive.Value);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
                filteredNotes = filteredNotes.Where(n => n.Content.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));

            if (filter.CreatedFrom.HasValue)
                filteredNotes = filteredNotes.Where(n => n.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                filteredNotes = filteredNotes.Where(n => n.CreatedAt <= filter.CreatedTo.Value);

            var totalCount = filteredNotes.Count();
            var notes = filteredNotes
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .Select(n => new NoteDto
                {
                    Id = n.Id,
                    CompanyId = n.CompanyId,
                    CompanyName = n.Company?.Name,
                    ContactId = n.ContactId,
                    ContactName = n.Contact != null ? $"{n.Contact.FirstName} {n.Contact.LastName}" : null,
                    LeadId = n.LeadId,
                    LeadName = n.Lead?.Title,
                    OpportunityId = n.OpportunityId,
                    OpportunityTitle = n.Opportunity?.Title,
                    Content = n.Content,
                    IsPinned = n.IsPinned,
                    IsActive = n.IsActive,
                    CreatedAt = n.CreatedAt,
                    UpdatedAt = n.UpdatedAt,
                    CreatedBy = n.CreatedBy,
                    UpdatedBy = n.UpdatedBy,
                    IsDeleted = n.IsDeleted
                })
                .ToList();

            var pagedResult = new PagedResult<NoteDto>
            {
                Items = notes,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(new ApiResponse<PagedResult<NoteDto>>
            {
                Success = true,
                Message = "Notes retrieved successfully",
                Data = pagedResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes");
            return StatusCode(500, new ApiResponse<PagedResult<NoteDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving notes",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<NoteDto>>> GetById(Guid id)
    {
        try
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id);
            
            if (note == null || note.IsDeleted)
            {
                return NotFound(new ApiResponse<NoteDto>
                {
                    Success = false,
                    Message = "Note not found"
                });
            }

            var noteDto = new NoteDto
            {
                Id = note.Id,
                CompanyId = note.CompanyId,
                CompanyName = note.Company?.Name,
                ContactId = note.ContactId,
                ContactName = note.Contact != null ? $"{note.Contact.FirstName} {note.Contact.LastName}" : null,
                LeadId = note.LeadId,
                LeadName = note.Lead?.Title,
                OpportunityId = note.OpportunityId,
                OpportunityTitle = note.Opportunity?.Title,
                Content = note.Content,
                IsPinned = note.IsPinned,
                IsActive = note.IsActive,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                CreatedBy = note.CreatedBy,
                UpdatedBy = note.UpdatedBy,
                IsDeleted = note.IsDeleted
            };

            return Ok(new ApiResponse<NoteDto>
            {
                Success = true,
                Message = "Note retrieved successfully",
                Data = noteDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving note {Id}", id);
            return StatusCode(500, new ApiResponse<NoteDto>
            {
                Success = false,
                Message = "An error occurred while retrieving note",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<NoteDto>>> Create([FromBody] CreateNoteDto dto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            var note = new Note
            {
                CompanyId = dto.CompanyId,
                ContactId = dto.ContactId,
                LeadId = dto.LeadId,
                OpportunityId = dto.OpportunityId,
                Content = dto.Content,
                IsPinned = dto.IsPinned,
                IsActive = dto.IsActive,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Notes.AddAsync(note);
            await _unitOfWork.SaveChangesAsync();

            var noteDto = new NoteDto
            {
                Id = note.Id,
                CompanyId = note.CompanyId,
                ContactId = note.ContactId,
                LeadId = note.LeadId,
                OpportunityId = note.OpportunityId,
                Content = note.Content,
                IsPinned = note.IsPinned,
                IsActive = note.IsActive,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                CreatedBy = note.CreatedBy,
                UpdatedBy = note.UpdatedBy,
                IsDeleted = note.IsDeleted
            };

            return CreatedAtAction(nameof(GetById), new { id = note.Id }, new ApiResponse<NoteDto>
            {
                Success = true,
                Message = "Note created successfully",
                Data = noteDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating note");
            return StatusCode(500, new ApiResponse<NoteDto>
            {
                Success = false,
                Message = "An error occurred while creating note",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<NoteDto>>> Update(Guid id, [FromBody] UpdateNoteDto dto)
    {
        try
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id);
            
            if (note == null || note.IsDeleted)
            {
                return NotFound(new ApiResponse<NoteDto>
                {
                    Success = false,
                    Message = "Note not found"
                });
            }

            var currentUserId = GetCurrentUserId();
            
            note.Content = dto.Content;
            note.IsPinned = dto.IsPinned;
            note.IsActive = dto.IsActive;
            note.UpdatedBy = currentUserId;
            note.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Notes.Update(note);
            await _unitOfWork.SaveChangesAsync();

            var noteDto = new NoteDto
            {
                Id = note.Id,
                CompanyId = note.CompanyId,
                ContactId = note.ContactId,
                LeadId = note.LeadId,
                OpportunityId = note.OpportunityId,
                Content = note.Content,
                IsPinned = note.IsPinned,
                IsActive = note.IsActive,
                CreatedAt = note.CreatedAt,
                UpdatedAt = note.UpdatedAt,
                CreatedBy = note.CreatedBy,
                UpdatedBy = note.UpdatedBy,
                IsDeleted = note.IsDeleted
            };

            return Ok(new ApiResponse<NoteDto>
            {
                Success = true,
                Message = "Note updated successfully",
                Data = noteDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating note {Id}", id);
            return StatusCode(500, new ApiResponse<NoteDto>
            {
                Success = false,
                Message = "An error occurred while updating note",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            var note = await _unitOfWork.Notes.GetByIdAsync(id);
            
            if (note == null || note.IsDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Note not found"
                });
            }

            var currentUserId = GetCurrentUserId();
            
            note.IsDeleted = true;
            note.UpdatedBy = currentUserId;
            note.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.Notes.Update(note);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Note deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting note {Id}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting note",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

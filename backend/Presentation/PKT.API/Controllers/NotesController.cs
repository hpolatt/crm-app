using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PKT.Application.DTOs;
using PKT.Application.DTOs.Notes;
using PKT.Application.Interfaces;
using PKT.Domain.Entities;

namespace PKT.API.Controllers;

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
    public async Task<ActionResult<ApiResponse<List<NoteDto>>>> GetAll(
        [FromQuery] Guid? companyId = null,
        [FromQuery] Guid? contactId = null,
        [FromQuery] Guid? leadId = null,
        [FromQuery] Guid? opportunityId = null,
        [FromQuery] bool? isPinned = null)
    {
        try
        {
            var allNotes = await _unitOfWork.Notes.GetAllAsync();
            var filteredNotes = allNotes.Where(n => !n.IsDeleted && n.IsActive);

            if (companyId.HasValue)
            {
                filteredNotes = filteredNotes.Where(n => n.CompanyId == companyId.Value);
            }

            if (contactId.HasValue)
            {
                filteredNotes = filteredNotes.Where(n => n.ContactId == contactId.Value);
            }

            if (leadId.HasValue)
            {
                filteredNotes = filteredNotes.Where(n => n.LeadId == leadId.Value);
            }

            if (opportunityId.HasValue)
            {
                filteredNotes = filteredNotes.Where(n => n.OpportunityId == opportunityId.Value);
            }

            if (isPinned.HasValue)
            {
                filteredNotes = filteredNotes.Where(n => n.IsPinned == isPinned.Value);
            }

            var notes = filteredNotes
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(n => n.CreatedAt)
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

            return Ok(new ApiResponse<List<NoteDto>>
            {
                Success = true,
                Message = "Notes retrieved successfully",
                Data = notes
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notes");
            return StatusCode(500, new ApiResponse<List<NoteDto>>
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

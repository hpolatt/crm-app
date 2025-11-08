using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Contacts;
using CrmApp.Core.DTOs.Common;
using CrmApp.Core.Interfaces;
using CrmApp.Domain.Entities;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ContactsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ContactsController> _logger;

    public ContactsController(IUnitOfWork unitOfWork, ILogger<ContactsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ContactDto>>>> GetAll(
        [FromQuery] ContactFilterQuery filter,
        [FromQuery] PaginationQuery pagination)
    {
        try
        {
            var allContacts = await _unitOfWork.Contacts.GetAllAsync();
            var filteredContacts = allContacts.Where(c => !c.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                filteredContacts = filteredContacts.Where(c =>
                    c.FirstName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.LastName.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email != null && c.Email.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Phone != null && c.Phone.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Position != null && c.Position.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(filter.FirstName))
                filteredContacts = filteredContacts.Where(c => c.FirstName.Contains(filter.FirstName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.LastName))
                filteredContacts = filteredContacts.Where(c => c.LastName.Contains(filter.LastName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.Email))
                filteredContacts = filteredContacts.Where(c => c.Email != null && c.Email.Contains(filter.Email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.Phone))
                filteredContacts = filteredContacts.Where(c => c.Phone != null && c.Phone.Contains(filter.Phone));

            if (filter.CompanyId.HasValue)
                filteredContacts = filteredContacts.Where(c => c.CompanyId == filter.CompanyId.Value);

            if (filter.IsActive.HasValue)
                filteredContacts = filteredContacts.Where(c => c.IsActive == filter.IsActive.Value);

            if (filter.CreatedFrom.HasValue)
                filteredContacts = filteredContacts.Where(c => c.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                filteredContacts = filteredContacts.Where(c => c.CreatedAt <= filter.CreatedTo.Value);

            var totalCount = filteredContacts.Count();
            var contacts = filteredContacts
                .OrderByDescending(c => c.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    CompanyId = c.CompanyId,
                    CompanyName = c.Company != null ? c.Company.Name : string.Empty,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Mobile = c.Mobile,
                    Position = c.Position,
                    Department = c.Department,
                    Address = c.Address,
                    City = c.City,
                    Country = c.Country,
                    PostalCode = c.PostalCode,
                    BirthDate = c.BirthDate,
                    Notes = c.Notes,
                    IsPrimary = c.IsPrimary,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToList();

            var pagedResult = new PagedResult<ContactDto>
            {
                Items = contacts,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(new ApiResponse<PagedResult<ContactDto>>
            {
                Success = true,
                Message = "Contacts retrieved successfully",
                Data = pagedResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contacts");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving contacts",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> GetById(Guid id)
    {
        try
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            
            if (contact == null || contact.IsDeleted)
            {
                return NotFound(new ApiResponse<ContactDto>
                {
                    Success = false,
                    Message = "Contact not found"
                });
            }

            var contactDto = new ContactDto
            {
                Id = contact.Id,
                CompanyId = contact.CompanyId,
                CompanyName = contact.Company != null ? contact.Company.Name : string.Empty,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Position = contact.Position,
                Department = contact.Department,
                Address = contact.Address,
                City = contact.City,
                Country = contact.Country,
                PostalCode = contact.PostalCode,
                BirthDate = contact.BirthDate,
                Notes = contact.Notes,
                IsPrimary = contact.IsPrimary,
                IsActive = contact.IsActive,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            };

            return Ok(new ApiResponse<ContactDto>
            {
                Success = true,
                Message = "Contact retrieved successfully",
                Data = contactDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contact {ContactId}", id);
            return StatusCode(500, new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the contact",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("company/{companyId}")]
    public async Task<ActionResult<ApiResponse<List<ContactDto>>>> GetByCompany(Guid companyId)
    {
        try
        {
            var allContacts = await _unitOfWork.Contacts.GetAllAsync();
            var contacts = allContacts
                .Where(c => c.CompanyId == companyId && !c.IsDeleted)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.FirstName)
                .Select(c => new ContactDto
                {
                    Id = c.Id,
                    CompanyId = c.CompanyId,
                    CompanyName = c.Company != null ? c.Company.Name : string.Empty,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Mobile = c.Mobile,
                    Position = c.Position,
                    Department = c.Department,
                    Address = c.Address,
                    City = c.City,
                    Country = c.Country,
                    PostalCode = c.PostalCode,
                    BirthDate = c.BirthDate,
                    Notes = c.Notes,
                    IsPrimary = c.IsPrimary,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToList();

            return Ok(new ApiResponse<List<ContactDto>>
            {
                Success = true,
                Message = "Contacts retrieved successfully",
                Data = contacts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving contacts for company {CompanyId}", companyId);
            return StatusCode(500, new ApiResponse<List<ContactDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving contacts",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactDto>>> Create([FromBody] CreateContactDto createDto)
    {
        try
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<ContactDto>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            // If this is a primary contact, remove primary flag from other contacts of the same company
            if (createDto.IsPrimary && createDto.CompanyId.HasValue)
            {
                var companyContacts = await _unitOfWork.Contacts.GetAllAsync();
                var existingPrimaryContacts = companyContacts
                    .Where(c => c.CompanyId == createDto.CompanyId.Value && c.IsPrimary && !c.IsDeleted)
                    .ToList();

                foreach (var existingContact in existingPrimaryContacts)
                {
                    existingContact.IsPrimary = false;
                    _unitOfWork.Contacts.Update(existingContact);
                }
            }

            var contact = new Contact
            {
                CompanyId = createDto.CompanyId,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Email = createDto.Email,
                Phone = createDto.Phone,
                Mobile = createDto.Mobile,
                Position = createDto.Position,
                Department = createDto.Department,
                Address = createDto.Address,
                City = createDto.City,
                Country = createDto.Country,
                PostalCode = createDto.PostalCode,
                BirthDate = createDto.BirthDate,
                Notes = createDto.Notes,
                IsPrimary = createDto.IsPrimary,
                IsActive = true
            };

            await _unitOfWork.Contacts.AddAsync(contact);
            await _unitOfWork.SaveChangesAsync();

            var contactDto = new ContactDto
            {
                Id = contact.Id,
                CompanyId = contact.CompanyId,
                CompanyName = contact.Company != null ? contact.Company.Name : string.Empty,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Position = contact.Position,
                Department = contact.Department,
                Address = contact.Address,
                City = contact.City,
                Country = contact.Country,
                PostalCode = contact.PostalCode,
                BirthDate = contact.BirthDate,
                Notes = contact.Notes,
                IsPrimary = contact.IsPrimary,
                IsActive = contact.IsActive,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = contact.Id }, new ApiResponse<ContactDto>
            {
                Success = true,
                Message = "Contact created successfully",
                Data = contactDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating contact");
            return StatusCode(500, new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "An error occurred while creating the contact",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> Update(Guid id, [FromBody] UpdateContactDto updateDto)
    {
        try
        {
            // Validate model state
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<ContactDto>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            
            if (contact == null || contact.IsDeleted)
            {
                return NotFound(new ApiResponse<ContactDto>
                {
                    Success = false,
                    Message = "Contact not found"
                });
            }

            // If this is being set as primary contact, remove primary flag from other contacts of the same company
            if (updateDto.IsPrimary && updateDto.CompanyId.HasValue)
            {
                var companyContacts = await _unitOfWork.Contacts.GetAllAsync();
                var existingPrimaryContacts = companyContacts
                    .Where(c => c.CompanyId == updateDto.CompanyId.Value && c.Id != id && c.IsPrimary && !c.IsDeleted)
                    .ToList();

                foreach (var existingContact in existingPrimaryContacts)
                {
                    existingContact.IsPrimary = false;
                    _unitOfWork.Contacts.Update(existingContact);
                }
            }

            // Update contact properties
            contact.CompanyId = updateDto.CompanyId;
            contact.FirstName = updateDto.FirstName;
            contact.LastName = updateDto.LastName;
            contact.Email = updateDto.Email;
            contact.Phone = updateDto.Phone;
            contact.Mobile = updateDto.Mobile;
            contact.Position = updateDto.Position;
            contact.Department = updateDto.Department;
            contact.Address = updateDto.Address;
            contact.City = updateDto.City;
            contact.Country = updateDto.Country;
            contact.PostalCode = updateDto.PostalCode;
            contact.BirthDate = updateDto.BirthDate;
            contact.Notes = updateDto.Notes;
            contact.IsPrimary = updateDto.IsPrimary;

            _unitOfWork.Contacts.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            var contactDto = new ContactDto
            {
                Id = contact.Id,
                CompanyId = contact.CompanyId,
                CompanyName = contact.Company != null ? contact.Company.Name : string.Empty,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Position = contact.Position,
                Department = contact.Department,
                Address = contact.Address,
                City = contact.City,
                Country = contact.Country,
                PostalCode = contact.PostalCode,
                BirthDate = contact.BirthDate,
                Notes = contact.Notes,
                IsPrimary = contact.IsPrimary,
                IsActive = contact.IsActive,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            };

            return Ok(new ApiResponse<ContactDto>
            {
                Success = true,
                Message = "Contact updated successfully",
                Data = contactDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating contact {ContactId}", id);
            return StatusCode(500, new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "An error occurred while updating the contact",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            
            if (contact == null || contact.IsDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Contact not found"
                });
            }

            contact.IsDeleted = true;
            _unitOfWork.Contacts.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Contact deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting contact {ContactId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the contact",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> ToggleActive(Guid id)
    {
        try
        {
            var contact = await _unitOfWork.Contacts.GetByIdAsync(id);
            
            if (contact == null || contact.IsDeleted)
            {
                return NotFound(new ApiResponse<ContactDto>
                {
                    Success = false,
                    Message = "Contact not found"
                });
            }

            contact.IsActive = !contact.IsActive;
            _unitOfWork.Contacts.Update(contact);
            await _unitOfWork.SaveChangesAsync();

            var contactDto = new ContactDto
            {
                Id = contact.Id,
                CompanyId = contact.CompanyId,
                CompanyName = contact.Company != null ? contact.Company.Name : string.Empty,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Phone = contact.Phone,
                Mobile = contact.Mobile,
                Position = contact.Position,
                Department = contact.Department,
                Address = contact.Address,
                City = contact.City,
                Country = contact.Country,
                PostalCode = contact.PostalCode,
                BirthDate = contact.BirthDate,
                Notes = contact.Notes,
                IsPrimary = contact.IsPrimary,
                IsActive = contact.IsActive,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            };

            return Ok(new ApiResponse<ContactDto>
            {
                Success = true,
                Message = $"Contact {(contact.IsActive ? "activated" : "deactivated")} successfully",
                Data = contactDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling active status for contact {ContactId}", id);
            return StatusCode(500, new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "An error occurred while toggling contact status",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

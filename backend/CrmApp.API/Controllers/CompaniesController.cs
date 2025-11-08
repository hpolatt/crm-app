using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Companies;
using CrmApp.Core.DTOs.Common;
using CrmApp.Core.Interfaces;
using CrmApp.Domain.Entities;

namespace CrmApp.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompaniesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CompaniesController> _logger;

    public CompaniesController(IUnitOfWork unitOfWork, ILogger<CompaniesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<CompanyDto>>>> GetAll(
        [FromQuery] CompanyFilterQuery filter,
        [FromQuery] PaginationQuery pagination)
    {
        try
        {
            var allCompanies = await _unitOfWork.Companies.GetAllAsync();
            var filteredCompanies = allCompanies.Where(c => !c.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                filteredCompanies = filteredCompanies.Where(c =>
                    c.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email != null && c.Email.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Website != null && c.Website.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(filter.Name))
                filteredCompanies = filteredCompanies.Where(c => c.Name.Contains(filter.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filter.Industry))
                filteredCompanies = filteredCompanies.Where(c => c.Industry == filter.Industry);

            if (!string.IsNullOrEmpty(filter.Source))
                filteredCompanies = filteredCompanies.Where(c => c.Source == filter.Source);

            if (filter.IsActive.HasValue)
                filteredCompanies = filteredCompanies.Where(c => c.IsActive == filter.IsActive.Value);

            if (filter.CreatedFrom.HasValue)
                filteredCompanies = filteredCompanies.Where(c => c.CreatedAt >= filter.CreatedFrom.Value);

            if (filter.CreatedTo.HasValue)
                filteredCompanies = filteredCompanies.Where(c => c.CreatedAt <= filter.CreatedTo.Value);

            var totalCount = filteredCompanies.Count();
            var companies = filteredCompanies
                .OrderByDescending(c => c.CreatedAt)
                .Skip(pagination.Skip)
                .Take(pagination.PageSize)
                .Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Industry = c.Industry,
                    Website = c.Website,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    City = c.City,
                    Country = c.Country,
                    PostalCode = c.PostalCode,
                    Source = c.Source,
                    EmployeeCount = c.EmployeeCount,
                    AnnualRevenue = c.AnnualRevenue,
                    Notes = c.Notes,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToList();

            var pagedResult = new PagedResult<CompanyDto>
            {
                Items = companies,
                TotalCount = totalCount,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize
            };

            return Ok(new ApiResponse<PagedResult<CompanyDto>>
            {
                Success = true,
                Message = "Companies retrieved successfully",
                Data = pagedResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies");
            return StatusCode(500, new ApiResponse<PagedResult<CompanyDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving companies",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyDto>>> GetById(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            
            if (company == null || company.IsDeleted)
            {
                return NotFound(new ApiResponse<CompanyDto>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            var companyDto = new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Industry = company.Industry,
                Website = company.Website,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                City = company.City,
                Country = company.Country,
                PostalCode = company.PostalCode,
                Source = company.Source,
                EmployeeCount = company.EmployeeCount,
                AnnualRevenue = company.AnnualRevenue,
                Notes = company.Notes,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };

            return Ok(new ApiResponse<CompanyDto>
            {
                Success = true,
                Message = "Company retrieved successfully",
                Data = companyDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving company {CompanyId}", id);
            return StatusCode(500, new ApiResponse<CompanyDto>
            {
                Success = false,
                Message = "An error occurred while retrieving the company",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CompanyDto>>> Create([FromBody] CreateCompanyDto createDto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            var company = new Company
            {
                Name = createDto.Name,
                Industry = createDto.Industry,
                Website = createDto.Website,
                Phone = createDto.Phone,
                Email = createDto.Email,
                Address = createDto.Address,
                City = createDto.City,
                Country = createDto.Country,
                PostalCode = createDto.PostalCode,
                Source = createDto.Source,
                EmployeeCount = createDto.EmployeeCount,
                AnnualRevenue = createDto.AnnualRevenue,
                Notes = createDto.Notes,
                IsActive = true,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Companies.AddAsync(company);
            await _unitOfWork.SaveChangesAsync();

            var companyDto = new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Industry = company.Industry,
                Website = company.Website,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                City = company.City,
                Country = company.Country,
                PostalCode = company.PostalCode,
                Source = company.Source,
                EmployeeCount = company.EmployeeCount,
                AnnualRevenue = company.AnnualRevenue,
                Notes = company.Notes,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = company.Id }, new ApiResponse<CompanyDto>
            {
                Success = true,
                Message = "Company created successfully",
                Data = companyDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating company");
            return StatusCode(500, new ApiResponse<CompanyDto>
            {
                Success = false,
                Message = "An error occurred while creating the company",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyDto>>> Update(Guid id, [FromBody] UpdateCompanyDto updateDto)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            
            if (company == null || company.IsDeleted)
            {
                return NotFound(new ApiResponse<CompanyDto>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            if (!string.IsNullOrEmpty(updateDto.Name))
                company.Name = updateDto.Name;
            if (updateDto.Industry != null)
                company.Industry = updateDto.Industry;
            if (updateDto.Website != null)
                company.Website = updateDto.Website;
            if (updateDto.Phone != null)
                company.Phone = updateDto.Phone;
            if (updateDto.Email != null)
                company.Email = updateDto.Email;
            if (updateDto.Address != null)
                company.Address = updateDto.Address;
            if (updateDto.City != null)
                company.City = updateDto.City;
            if (updateDto.Country != null)
                company.Country = updateDto.Country;
            if (updateDto.PostalCode != null)
                company.PostalCode = updateDto.PostalCode;
            if (updateDto.Source != null)
                company.Source = updateDto.Source;
            if (updateDto.EmployeeCount.HasValue)
                company.EmployeeCount = updateDto.EmployeeCount;
            if (updateDto.AnnualRevenue.HasValue)
                company.AnnualRevenue = updateDto.AnnualRevenue;
            if (updateDto.Notes != null)
                company.Notes = updateDto.Notes;
            if (updateDto.IsActive.HasValue)
                company.IsActive = updateDto.IsActive.Value;

            var currentUserId = GetCurrentUserId();
            company.UpdatedBy = currentUserId;
            company.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync();

            var companyDto = new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Industry = company.Industry,
                Website = company.Website,
                Phone = company.Phone,
                Email = company.Email,
                Address = company.Address,
                City = company.City,
                Country = company.Country,
                PostalCode = company.PostalCode,
                Source = company.Source,
                EmployeeCount = company.EmployeeCount,
                AnnualRevenue = company.AnnualRevenue,
                Notes = company.Notes,
                IsActive = company.IsActive,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };

            return Ok(new ApiResponse<CompanyDto>
            {
                Success = true,
                Message = "Company updated successfully",
                Data = companyDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating company {CompanyId}", id);
            return StatusCode(500, new ApiResponse<CompanyDto>
            {
                Success = false,
                Message = "An error occurred while updating the company",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            
            if (company == null || company.IsDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Company not found"
                });
            }

            var currentUserId = GetCurrentUserId();
            company.IsDeleted = true;
            company.UpdatedBy = currentUserId;
            company.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.Companies.Update(company);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Company deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting company {CompanyId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the company",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

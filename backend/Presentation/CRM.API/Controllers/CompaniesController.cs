using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CRM.Application.DTOs;
using CRM.Application.DTOs.Companies;
using CRM.Application.Interfaces;
using CRM.Domain.Entities;

namespace CRM.API.Controllers;

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
    public async Task<ActionResult<ApiResponse<object>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] string? industry = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var allCompanies = await _unitOfWork.Companies.GetAllAsync();
            var filteredCompanies = allCompanies.Where(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                filteredCompanies = filteredCompanies.Where(c =>
                    c.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (c.Email != null && c.Email.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Website != null && c.Website.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(industry))
            {
                filteredCompanies = filteredCompanies.Where(c => c.Industry == industry);
            }

            if (isActive.HasValue)
            {
                filteredCompanies = filteredCompanies.Where(c => c.IsActive == isActive.Value);
            }

            var totalCount = filteredCompanies.Count();
            var companies = filteredCompanies
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Companies retrieved successfully",
                Data = new
                {
                    items = companies,
                    totalCount,
                    pageNumber,
                    pageSize,
                    totalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving companies");
            return StatusCode(500, new ApiResponse<object>
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

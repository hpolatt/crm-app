using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PKT.Application.DTOs;
using PKT.Application.DTOs.Settings;
using PKT.Application.Interfaces;
using PKT.Domain.Entities;

namespace PKT.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SettingsController> _logger;

    public SettingsController(IUnitOfWork unitOfWork, ILogger<SettingsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<SystemSettingDto>>>> GetAll()
    {
        try
        {
            var settings = await _unitOfWork.SystemSettings.GetAllAsync();
            var settingsList = settings
                .Where(s => !s.IsDeleted)
                .Select(s => new SystemSettingDto
                {
                    Id = s.Id,
                    Key = s.Key,
                    Value = s.Value,
                    Description = s.Description,
                    DataType = s.DataType,
                    Category = s.Category,
                    IsPublic = s.IsPublic,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToList();

            return Ok(ApiResponse<List<SystemSettingDto>>.SuccessResponse(
                settingsList, 
                "Settings retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving settings");
            return StatusCode(500, ApiResponse<List<SystemSettingDto>>.ErrorResponse(
                "An error occurred while retrieving settings",
                new List<string> { ex.Message }
            ));
        }
    }

    [HttpGet("{key}")]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> GetByKey(string key)
    {
        try
        {
            var settings = await _unitOfWork.SystemSettings.GetAllAsync();
            var setting = settings.FirstOrDefault(s => s.Key == key && !s.IsDeleted);

            if (setting == null)
            {
                return NotFound(ApiResponse<SystemSettingDto>.ErrorResponse(
                    $"Setting with key '{key}' not found",
                    new List<string> { "Setting not found" }
                ));
            }

            var settingDto = new SystemSettingDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                DataType = setting.DataType,
                Category = setting.Category,
                IsPublic = setting.IsPublic,
                CreatedAt = setting.CreatedAt,
                UpdatedAt = setting.UpdatedAt
            };

            return Ok(ApiResponse<SystemSettingDto>.SuccessResponse(
                settingDto, 
                "Setting retrieved successfully"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving setting");
            return StatusCode(500, ApiResponse<SystemSettingDto>.ErrorResponse(
                "An error occurred while retrieving setting",
                new List<string> { ex.Message }
            ));
        }
    }

    [HttpPut("{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<SystemSettingDto>>> Update(
        string key, 
        [FromBody] UpdateSystemSettingDto dto)
    {
        try
        {
            var settings = await _unitOfWork.SystemSettings.GetAllAsync();
            var setting = settings.FirstOrDefault(s => s.Key == key && !s.IsDeleted);

            if (setting == null)
            {
                // Create new setting
                setting = new SystemSetting
                {
                    Key = dto.Key,
                    Value = dto.Value,
                    Description = dto.Description,
                    DataType = dto.DataType,
                    Category = dto.Category,
                    IsPublic = dto.IsPublic
                };

                await _unitOfWork.SystemSettings.AddAsync(setting);
            }
            else
            {
                // Update existing setting
                setting.Value = dto.Value;
                setting.Description = dto.Description;
                setting.DataType = dto.DataType;
                setting.Category = dto.Category;
                setting.IsPublic = dto.IsPublic;

                _unitOfWork.SystemSettings.Update(setting);
            }

            await _unitOfWork.SaveChangesAsync();

            var settingDto = new SystemSettingDto
            {
                Id = setting.Id,
                Key = setting.Key,
                Value = setting.Value,
                Description = setting.Description,
                DataType = setting.DataType,
                Category = setting.Category,
                IsPublic = setting.IsPublic,
                CreatedAt = setting.CreatedAt,
                UpdatedAt = setting.UpdatedAt
            };

            return Ok(ApiResponse<SystemSettingDto>.SuccessResponse(
                settingDto, 
                "Setting updated successfully"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating setting");
            return StatusCode(500, ApiResponse<SystemSettingDto>.ErrorResponse(
                "An error occurred while updating setting",
                new List<string> { ex.Message }
            ));
        }
    }

    [HttpDelete("{key}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(string key)
    {
        try
        {
            var settings = await _unitOfWork.SystemSettings.GetAllAsync();
            var setting = settings.FirstOrDefault(s => s.Key == key && !s.IsDeleted);

            if (setting == null)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(
                    $"Setting with key '{key}' not found",
                    new List<string> { "Setting not found" }
                ));
            }

            setting.IsDeleted = true;
            _unitOfWork.SystemSettings.Update(setting);
            await _unitOfWork.SaveChangesAsync();

            return Ok(ApiResponse<object>.SuccessResponse(
                new { },
                "Setting deleted successfully"
            ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting setting");
            return StatusCode(500, ApiResponse<object>.ErrorResponse(
                "An error occurred while deleting setting",
                new List<string> { ex.Message }
            ));
        }
    }
}

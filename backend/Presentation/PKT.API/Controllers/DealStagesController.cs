using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PKT.Application.DTOs;
using PKT.Application.DTOs.DealStages;
using PKT.Application.Interfaces;
using PKT.Domain.Entities;

namespace PKT.API.Controllers;

[Authorize]
[ApiController]
[Route("api/deal-stages")]
public class DealStagesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DealStagesController> _logger;

    public DealStagesController(IUnitOfWork unitOfWork, ILogger<DealStagesController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<DealStageDto>>>> GetAll([FromQuery] bool? isActive = null)
    {
        try
        {
            var allStages = await _unitOfWork.DealStages.GetAllAsync();
            var stages = allStages.Where(s => !s.IsDeleted);

            if (isActive.HasValue)
            {
                stages = stages.Where(s => s.IsActive == isActive.Value);
            }

            var stageDtos = stages
                .OrderBy(s => s.Order)
                .Select(s => new DealStageDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Order = s.Order,
                    Description = s.Description,
                    Color = s.Color,
                    IsDefault = s.IsDefault,
                    IsActive = s.IsActive,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt
                })
                .ToList();

            return Ok(new ApiResponse<List<DealStageDto>>
            {
                Success = true,
                Message = "Deal stages retrieved successfully",
                Data = stageDtos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deal stages");
            return StatusCode(500, new ApiResponse<List<DealStageDto>>
            {
                Success = false,
                Message = "An error occurred while retrieving deal stages",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DealStageDto>>> GetById(Guid id)
    {
        try
        {
            var stage = await _unitOfWork.DealStages.GetByIdAsync(id);
            
            if (stage == null || stage.IsDeleted)
            {
                return NotFound(new ApiResponse<DealStageDto>
                {
                    Success = false,
                    Message = "Deal stage not found"
                });
            }

            var stageDto = new DealStageDto
            {
                Id = stage.Id,
                Name = stage.Name,
                Order = stage.Order,
                Description = stage.Description,
                Color = stage.Color,
                IsDefault = stage.IsDefault,
                IsActive = stage.IsActive,
                CreatedAt = stage.CreatedAt,
                UpdatedAt = stage.UpdatedAt
            };

            return Ok(new ApiResponse<DealStageDto>
            {
                Success = true,
                Message = "Deal stage retrieved successfully",
                Data = stageDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving deal stage {Id}", id);
            return StatusCode(500, new ApiResponse<DealStageDto>
            {
                Success = false,
                Message = "An error occurred while retrieving deal stage",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DealStageDto>>> Create([FromBody] CreateDealStageDto dto)
    {
        try
        {
            var stage = new DealStage
            {
                Name = dto.Name,
                Order = dto.Order,
                Description = dto.Description,
                Color = dto.Color,
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive
            };

            await _unitOfWork.DealStages.AddAsync(stage);
            await _unitOfWork.SaveChangesAsync();

            var stageDto = new DealStageDto
            {
                Id = stage.Id,
                Name = stage.Name,
                Order = stage.Order,
                Description = stage.Description,
                Color = stage.Color,
                IsDefault = stage.IsDefault,
                IsActive = stage.IsActive,
                CreatedAt = stage.CreatedAt,
                UpdatedAt = stage.UpdatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = stage.Id }, new ApiResponse<DealStageDto>
            {
                Success = true,
                Message = "Deal stage created successfully",
                Data = stageDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating deal stage");
            return StatusCode(500, new ApiResponse<DealStageDto>
            {
                Success = false,
                Message = "An error occurred while creating deal stage",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<DealStageDto>>> Update(Guid id, [FromBody] UpdateDealStageDto dto)
    {
        try
        {
            var stage = await _unitOfWork.DealStages.GetByIdAsync(id);
            
            if (stage == null || stage.IsDeleted)
            {
                return NotFound(new ApiResponse<DealStageDto>
                {
                    Success = false,
                    Message = "Deal stage not found"
                });
            }

            stage.Name = dto.Name;
            stage.Order = dto.Order;
            stage.Description = dto.Description;
            stage.Color = dto.Color;
            stage.IsDefault = dto.IsDefault;
            stage.IsActive = dto.IsActive;

            _unitOfWork.DealStages.Update(stage);
            await _unitOfWork.SaveChangesAsync();

            var stageDto = new DealStageDto
            {
                Id = stage.Id,
                Name = stage.Name,
                Order = stage.Order,
                Description = stage.Description,
                Color = stage.Color,
                IsDefault = stage.IsDefault,
                IsActive = stage.IsActive,
                CreatedAt = stage.CreatedAt,
                UpdatedAt = stage.UpdatedAt
            };

            return Ok(new ApiResponse<DealStageDto>
            {
                Success = true,
                Message = "Deal stage updated successfully",
                Data = stageDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating deal stage {Id}", id);
            return StatusCode(500, new ApiResponse<DealStageDto>
            {
                Success = false,
                Message = "An error occurred while updating deal stage",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        try
        {
            var stage = await _unitOfWork.DealStages.GetByIdAsync(id);
            
            if (stage == null || stage.IsDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Deal stage not found"
                });
            }

            stage.IsDeleted = true;
            _unitOfWork.DealStages.Update(stage);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Deal stage deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting deal stage {Id}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting deal stage",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}

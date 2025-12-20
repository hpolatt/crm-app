using Microsoft.AspNetCore.Mvc;
using PktApp.Core.DTOs.Common;
using PktApp.Core.DTOs.DelayReasons;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DelayReasonsController : BaseController
{
    private readonly IRepository<DelayReason> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DelayReasonsController(IRepository<DelayReason> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<DelayReasonDto>>>> GetAll()
    {
        var delayReasons = await _repository.GetAllAsync();
        var dtos = delayReasons.Select(dr => new DelayReasonDto
        {
            Id = dr.Id,
            Name = dr.Name,
            CreatedAt = dr.CreatedAt,
            UpdatedAt = dr.UpdatedAt
        });
        return Ok(ApiResponse<IEnumerable<DelayReasonDto>>.SuccessResponse(dtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<DelayReasonDto>>> GetById(Guid id)
    {
        var delayReason = await _repository.GetByIdAsync(id);
        if (delayReason == null)
            return NotFound(ApiResponse<DelayReasonDto>.ErrorResponse("Delay reason not found"));

        var dto = new DelayReasonDto
        {
            Id = delayReason.Id,
            Name = delayReason.Name,
            CreatedAt = delayReason.CreatedAt,
            UpdatedAt = delayReason.UpdatedAt
        };
        return Ok(ApiResponse<DelayReasonDto>.SuccessResponse(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<DelayReasonDto>>> Create(CreateDelayReasonDto createDto)
    {
        var delayReason = new DelayReason
        {
            Name = createDto.Name
        };

        await _repository.AddAsync(delayReason);
        await _unitOfWork.CommitAsync();

        var dto = new DelayReasonDto
        {
            Id = delayReason.Id,
            Name = delayReason.Name,
            CreatedAt = delayReason.CreatedAt,
            UpdatedAt = delayReason.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<DelayReasonDto>.SuccessResponse(dto));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<DelayReasonDto>>> Update(Guid id, UpdateDelayReasonDto updateDto)
    {
        var delayReason = await _repository.GetByIdAsync(id);
        if (delayReason == null)
            return NotFound(ApiResponse<DelayReasonDto>.ErrorResponse("Delay reason not found"));

        delayReason.Name = updateDto.Name;
        _repository.Update(delayReason);
        await _unitOfWork.CommitAsync();

        var dto = new DelayReasonDto
        {
            Id = delayReason.Id,
            Name = delayReason.Name,
            CreatedAt = delayReason.CreatedAt,
            UpdatedAt = delayReason.UpdatedAt
        };

        return Ok(ApiResponse<DelayReasonDto>.SuccessResponse(dto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var delayReason = await _repository.GetByIdAsync(id);
        if (delayReason == null)
            return NotFound(ApiResponse<bool>.ErrorResponse("Delay reason not found"));

        _repository.Remove(delayReason);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}

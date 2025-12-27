using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PktApp.Core.DTOs.Common;
using PktApp.Core.DTOs.Reactors;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReactorsController : BaseController
{
    private readonly IRepository<Reactor> _repository;
    private readonly IRepository<PktTransaction> _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReactorsController(
        IRepository<Reactor> repository, 
        IRepository<PktTransaction> transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReactorDto>>>> GetAll()
    {
        var reactors = await _repository.GetAllAsync();
        var dtos = reactors.Select(r => new ReactorDto
        {
            Id = r.Id,
            Name = r.Name,
            CreatedAt = r.CreatedAt,
            UpdatedAt = r.UpdatedAt
        });
        return Ok(ApiResponse<IEnumerable<ReactorDto>>.SuccessResponse(dtos));
    }

    [HttpGet("usage-analysis")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ReactorUsageAnalysisDto>>>> GetUsageAnalysis(
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null)
    {
        IQueryable<PktTransaction> transactionQuery = _transactionRepository.GetQueryable()
            .Include(t => t.Reactor);

        // Apply date filters
        if (startDateFrom.HasValue)
        {
            transactionQuery = transactionQuery.Where(t => t.StartOfWork.HasValue && t.StartOfWork.Value >= startDateFrom.Value);
        }

        if (startDateTo.HasValue)
        {
            var endOfDay = startDateTo.Value.Date.AddDays(1).AddTicks(-1);
            transactionQuery = transactionQuery.Where(t => t.StartOfWork.HasValue && t.StartOfWork.Value <= endOfDay);
        }

        // Get filtered transactions
        var transactions = await transactionQuery.ToListAsync();

        // Group by reactor and calculate aggregates
        var results = transactions
            .GroupBy(t => new { t.ReactorId, ReactorName = t.Reactor?.Name ?? "Unknown" })
            .Select(g => new ReactorUsageAnalysisDto
            {
                ReactorId = g.Key.ReactorId,
                ReactorName = g.Key.ReactorName,
                TransactionCount = g.Count(),
                TotalProductionDuration = TimeSpan.FromTicks(g.Sum(t => t.ActualProductionDuration?.Ticks ?? 0)),
                TotalWashingDuration = TimeSpan.FromTicks(g.Sum(t => t.WashingDuration?.Ticks ?? 0)),
                TotalDelayDuration = TimeSpan.FromTicks(g.Sum(t => t.DelayDuration?.Ticks ?? 0))
            })
            .ToList();

        return Ok(ApiResponse<IEnumerable<ReactorUsageAnalysisDto>>.SuccessResponse(results));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ReactorDto>>> GetById(Guid id)
    {
        var reactor = await _repository.GetByIdAsync(id);
        if (reactor == null)
            return NotFound(ApiResponse<ReactorDto>.ErrorResponse("Reactor not found"));

        var dto = new ReactorDto
        {
            Id = reactor.Id,
            Name = reactor.Name,
            CreatedAt = reactor.CreatedAt,
            UpdatedAt = reactor.UpdatedAt
        };
        return Ok(ApiResponse<ReactorDto>.SuccessResponse(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReactorDto>>> Create(CreateReactorDto createDto)
    {
        // Foreman ekleme yapamaz
        if (IsForeman())
        {
            return ForbiddenResponse<ReactorDto>();
        }

        var reactor = new Reactor
        {
            Name = createDto.Name
        };

        await _repository.AddAsync(reactor);
        await _unitOfWork.CommitAsync();

        var dto = new ReactorDto
        {
            Id = reactor.Id,
            Name = reactor.Name,
            CreatedAt = reactor.CreatedAt,
            UpdatedAt = reactor.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<ReactorDto>.SuccessResponse(dto));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ReactorDto>>> Update(Guid id, UpdateReactorDto updateDto)
    {
        // Foreman düzenleme yapamaz
        if (IsForeman())
        {
            return ForbiddenResponse<ReactorDto>();
        }

        var reactor = await _repository.GetByIdAsync(id);
        if (reactor == null)
            return NotFound(ApiResponse<ReactorDto>.ErrorResponse("Reactor not found"));

        reactor.Name = updateDto.Name;
        _repository.Update(reactor);
        await _unitOfWork.CommitAsync();

        var dto = new ReactorDto
        {
            Id = reactor.Id,
            Name = reactor.Name,
            CreatedAt = reactor.CreatedAt,
            UpdatedAt = reactor.UpdatedAt
        };

        return Ok(ApiResponse<ReactorDto>.SuccessResponse(dto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var reactor = await _repository.GetByIdAsync(id);
        if (reactor == null)
            return NotFound(ApiResponse<bool>.ErrorResponse("Reactor not found"));

        _repository.Remove(reactor);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}

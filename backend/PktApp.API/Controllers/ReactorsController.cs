using Microsoft.AspNetCore.Mvc;
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
    private readonly IUnitOfWork _unitOfWork;

    public ReactorsController(IRepository<Reactor> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
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

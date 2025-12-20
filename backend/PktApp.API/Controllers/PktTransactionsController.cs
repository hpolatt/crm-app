using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PktApp.Core.DTOs.Common;
using PktApp.Core.DTOs.PktTransactions;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PktTransactionsController : BaseController
{
    private readonly IRepository<PktTransaction> _repository;
    private readonly IRepository<Reactor> _reactorRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IRepository<DelayReason> _delayReasonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PktTransactionsController(
        IRepository<PktTransaction> repository,
        IRepository<Reactor> reactorRepository,
        IRepository<Product> productRepository,
        IRepository<DelayReason> delayReasonRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _reactorRepository = reactorRepository;
        _productRepository = productRepository;
        _delayReasonRepository = delayReasonRepository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<PktTransactionDto>>>> GetAll()
    {
        var transactions = await _repository.GetAllAsync();
        var dtos = new List<PktTransactionDto>();

        foreach (var t in transactions)
        {
            var reactor = await _reactorRepository.GetByIdAsync(t.ReactorId);
            var product = await _productRepository.GetByIdAsync(t.ProductId);
            var delayReason = t.DelayReasonId.HasValue ? await _delayReasonRepository.GetByIdAsync(t.DelayReasonId.Value) : null;

            dtos.Add(new PktTransactionDto
            {
                Id = t.Id,
                Status = t.Status,
                ReactorId = t.ReactorId,
                ReactorName = reactor?.Name ?? string.Empty,
                ProductId = t.ProductId,
                ProductName = product?.ProductName ?? string.Empty,
                WorkOrderNo = t.WorkOrderNo,
                LotNo = t.LotNo,
                StartOfWork = t.StartOfWork,
                End = t.End,
                ActualProductionDuration = t.ActualProductionDuration,
                DelayDuration = t.DelayDuration,
                WashingDuration = t.WashingDuration,
                CausticAmountKg = t.CausticAmountKg,
                DelayReasonId = t.DelayReasonId,
                DelayReasonName = delayReason?.Name,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            });
        }

        return Ok(ApiResponse<IEnumerable<PktTransactionDto>>.SuccessResponse(dtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<PktTransactionDto>>> GetById(Guid id)
    {
        var transaction = await _repository.GetByIdAsync(id);
        if (transaction == null)
            return NotFound(ApiResponse<PktTransactionDto>.ErrorResponse("Transaction not found"));

        var reactor = await _reactorRepository.GetByIdAsync(transaction.ReactorId);
        var product = await _productRepository.GetByIdAsync(transaction.ProductId);
        var delayReason = transaction.DelayReasonId.HasValue ? await _delayReasonRepository.GetByIdAsync(transaction.DelayReasonId.Value) : null;

        var dto = new PktTransactionDto
        {
            Id = transaction.Id,
            Status = transaction.Status,
            ReactorId = transaction.ReactorId,
            ReactorName = reactor?.Name ?? string.Empty,
            ProductId = transaction.ProductId,
            ProductName = product?.ProductName ?? string.Empty,
            WorkOrderNo = transaction.WorkOrderNo,
            LotNo = transaction.LotNo,
            StartOfWork = transaction.StartOfWork,
            End = transaction.End,
            ActualProductionDuration = transaction.ActualProductionDuration,
            DelayDuration = transaction.DelayDuration,
            WashingDuration = transaction.WashingDuration,
            CausticAmountKg = transaction.CausticAmountKg,
            DelayReasonId = transaction.DelayReasonId,
            DelayReasonName = delayReason?.Name,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return Ok(ApiResponse<PktTransactionDto>.SuccessResponse(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<PktTransactionDto>>> Create([FromBody] CreatePktTransactionDto createDto)
    {
        var transaction = new PktTransaction
        {
            Status = Domain.Enums.TransactionStatus.Planned, // Otomatik olarak Planned
            ReactorId = createDto.ReactorId,
            ProductId = createDto.ProductId,
            WorkOrderNo = createDto.WorkOrderNo,
            LotNo = createDto.LotNo,
            // StartOfWork status değiştiğinde ayarlanacak
            CausticAmountKg = createDto.CausticAmountKg,
            DelayReasonId = createDto.DelayReasonId,
            Description = createDto.Description
        };

        await _repository.AddAsync(transaction);
        await _unitOfWork.CommitAsync();

        var reactor = await _reactorRepository.GetByIdAsync(transaction.ReactorId);
        var product = await _productRepository.GetByIdAsync(transaction.ProductId);
        var delayReason = transaction.DelayReasonId.HasValue ? await _delayReasonRepository.GetByIdAsync(transaction.DelayReasonId.Value) : null;

        var dto = new PktTransactionDto
        {
            Id = transaction.Id,
            Status = transaction.Status,
            ReactorId = transaction.ReactorId,
            ReactorName = reactor?.Name ?? string.Empty,
            ProductId = transaction.ProductId,
            ProductName = product?.ProductName ?? string.Empty,
            WorkOrderNo = transaction.WorkOrderNo,
            LotNo = transaction.LotNo,
            StartOfWork = transaction.StartOfWork,
            End = transaction.End,
            ActualProductionDuration = transaction.ActualProductionDuration,
            DelayDuration = transaction.DelayDuration,
            WashingDuration = transaction.WashingDuration,
            CausticAmountKg = transaction.CausticAmountKg,
            DelayReasonId = transaction.DelayReasonId,
            DelayReasonName = delayReason?.Name,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<PktTransactionDto>.SuccessResponse(dto));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<PktTransactionDto>>> Update(Guid id, UpdatePktTransactionDto updateDto)
    {
        // TODO: Admin kontrolü eklenecek - şimdilik mock
        // if (!User.IsInRole("Admin")) return Forbid();
        
        var transaction = await _repository.GetByIdAsync(id);
        if (transaction == null)
            return NotFound(ApiResponse<PktTransactionDto>.ErrorResponse("Transaction not found"));

        transaction.Status = updateDto.Status;
        transaction.ReactorId = updateDto.ReactorId;
        transaction.ProductId = updateDto.ProductId;
        transaction.WorkOrderNo = updateDto.WorkOrderNo;
        transaction.LotNo = updateDto.LotNo;
        transaction.StartOfWork = updateDto.StartOfWork;
        transaction.End = updateDto.End;
        transaction.ActualProductionDuration = updateDto.ActualProductionDuration;
        transaction.DelayDuration = updateDto.DelayDuration;
        transaction.WashingDuration = updateDto.WashingDuration;
        transaction.CausticAmountKg = updateDto.CausticAmountKg;
        transaction.DelayReasonId = updateDto.DelayReasonId;
        transaction.Description = updateDto.Description;
        transaction.UpdatedAt = DateTime.UtcNow; // UpdatedAt güncelle

        _repository.Update(transaction);
        await _unitOfWork.CommitAsync();

        var reactor = await _reactorRepository.GetByIdAsync(transaction.ReactorId);
        var product = await _productRepository.GetByIdAsync(transaction.ProductId);
        var delayReason = transaction.DelayReasonId.HasValue ? await _delayReasonRepository.GetByIdAsync(transaction.DelayReasonId.Value) : null;

        var dto = new PktTransactionDto
        {
            Id = transaction.Id,
            Status = transaction.Status,
            ReactorId = transaction.ReactorId,
            ReactorName = reactor?.Name ?? string.Empty,
            ProductId = transaction.ProductId,
            ProductName = product?.ProductName ?? string.Empty,
            WorkOrderNo = transaction.WorkOrderNo,
            LotNo = transaction.LotNo,
            StartOfWork = transaction.StartOfWork,
            End = transaction.End,
            ActualProductionDuration = transaction.ActualProductionDuration,
            DelayDuration = transaction.DelayDuration,
            WashingDuration = transaction.WashingDuration,
            CausticAmountKg = transaction.CausticAmountKg,
            DelayReasonId = transaction.DelayReasonId,
            DelayReasonName = delayReason?.Name,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return Ok(ApiResponse<PktTransactionDto>.SuccessResponse(dto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var transaction = await _repository.GetByIdAsync(id);
        if (transaction == null)
            return NotFound(ApiResponse<bool>.ErrorResponse("Transaction not found"));

        _repository.Remove(transaction);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<PktTransactionDto>>> UpdateStatus(Guid id, [FromBody] TransactionStatusUpdateDto statusUpdate)
    {
        var transaction = await _repository.GetByIdAsync(id);
        if (transaction == null)
            return NotFound(ApiResponse<PktTransactionDto>.ErrorResponse("Transaction not found"));

        var oldStatus = transaction.Status;
        transaction.Status = statusUpdate.NewStatus;
        transaction.UpdatedAt = DateTime.UtcNow;

        // Status değişiminde StartOfWork'u ayarla
        if (oldStatus == Domain.Enums.TransactionStatus.Planned && 
            statusUpdate.NewStatus == Domain.Enums.TransactionStatus.InProgress)
        {
            transaction.StartOfWork = DateTime.UtcNow;
        }

        // Completed'a geçerken End zamanını ayarla
        if (statusUpdate.NewStatus == Domain.Enums.TransactionStatus.Completed && !transaction.End.HasValue)
        {
            transaction.End = DateTime.UtcNow;
            if (transaction.StartOfWork.HasValue)
            {
                transaction.ActualProductionDuration = transaction.End.Value - transaction.StartOfWork.Value;
            }
        }

        // Washing'e geçerken washing başlangıç zamanını kaydet (Description'a ekleyelim)
        if (statusUpdate.NewStatus == Domain.Enums.TransactionStatus.Washing)
        {
            // Washing başlangıç zamanı
        }

        // WashingCompleted'a geçerken washing süresini hesapla
        if (oldStatus == Domain.Enums.TransactionStatus.Washing && 
            statusUpdate.NewStatus == Domain.Enums.TransactionStatus.WashingCompleted)
        {
            // WashingDuration hesaplanabilir
        }

        // Not ekle
        if (!string.IsNullOrEmpty(statusUpdate.Note))
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var noteWithTimestamp = $"[{timestamp}] {statusUpdate.Note}";
            transaction.Description = string.IsNullOrEmpty(transaction.Description) 
                ? noteWithTimestamp 
                : transaction.Description + "\n" + noteWithTimestamp;
        }

        _repository.Update(transaction);
        await _unitOfWork.CommitAsync();

        var reactor = await _reactorRepository.GetByIdAsync(transaction.ReactorId);
        var product = await _productRepository.GetByIdAsync(transaction.ProductId);
        var delayReason = transaction.DelayReasonId.HasValue ? await _delayReasonRepository.GetByIdAsync(transaction.DelayReasonId.Value) : null;

        var dto = new PktTransactionDto
        {
            Id = transaction.Id,
            Status = transaction.Status,
            ReactorId = transaction.ReactorId,
            ReactorName = reactor?.Name ?? string.Empty,
            ProductId = transaction.ProductId,
            ProductName = product?.ProductName ?? string.Empty,
            WorkOrderNo = transaction.WorkOrderNo,
            LotNo = transaction.LotNo,
            StartOfWork = transaction.StartOfWork,
            End = transaction.End,
            ActualProductionDuration = transaction.ActualProductionDuration,
            DelayDuration = transaction.DelayDuration,
            WashingDuration = transaction.WashingDuration,
            CausticAmountKg = transaction.CausticAmountKg,
            DelayReasonId = transaction.DelayReasonId,
            DelayReasonName = delayReason?.Name,
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            UpdatedAt = transaction.UpdatedAt
        };

        return Ok(ApiResponse<PktTransactionDto>.SuccessResponse(dto));
    }
}

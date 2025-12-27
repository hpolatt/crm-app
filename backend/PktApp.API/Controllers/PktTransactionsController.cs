using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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
    public async Task<ActionResult<ApiResponse<IEnumerable<PktTransactionDto>>>> GetAll(
        [FromQuery] DateTime? startDateFrom = null,
        [FromQuery] DateTime? startDateTo = null,
        [FromQuery] Guid? reactorId = null,
        [FromQuery] Guid? productId = null,
        [FromQuery] List<string>? statuses = null,
        [FromQuery] string? workOrderNo = null,
        [FromQuery] string? lotNo = null)
    {
        // Start with IQueryable for database-level filtering
        var query = _repository.GetQueryable();

        // Apply filters at database level
        // For Planned transactions (StartOfWork is NULL), use CreatedAt for date filtering
        if (startDateFrom.HasValue)
        {
            query = query.Where(t => 
                (t.StartOfWork.HasValue && t.StartOfWork.Value >= startDateFrom.Value) ||
                (!t.StartOfWork.HasValue && t.CreatedAt >= startDateFrom.Value));
        }

        if (startDateTo.HasValue)
        {
            var endOfDay = startDateTo.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(t => 
                (t.StartOfWork.HasValue && t.StartOfWork.Value <= endOfDay) ||
                (!t.StartOfWork.HasValue && t.CreatedAt <= endOfDay));
        }

        if (reactorId.HasValue)
        {
            query = query.Where(t => t.ReactorId == reactorId.Value);
        }

        if (productId.HasValue)
        {
            query = query.Where(t => t.ProductId == productId.Value);
        }

        if (statuses != null && statuses.Any())
        {
            var statusEnums = new List<Domain.Enums.TransactionStatus>();
            foreach (var status in statuses)
            {
                if (Enum.TryParse<Domain.Enums.TransactionStatus>(status, out var statusEnum))
                {
                    statusEnums.Add(statusEnum);
                }
            }
            if (statusEnums.Any())
            {
                query = query.Where(t => statusEnums.Contains(t.Status));
            }
        }

        if (!string.IsNullOrWhiteSpace(workOrderNo))
        {
            query = query.Where(t => t.WorkOrderNo.Contains(workOrderNo));
        }

        if (!string.IsNullOrWhiteSpace(lotNo))
        {
            query = query.Where(t => t.LotNo.Contains(lotNo));
        }

        // Execute query and get filtered results from database
        var transactions = await query.ToListAsync();

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
                ProductCode = product?.ProductCode ?? string.Empty,
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
        var transaction = await _repository.GetByIdAsync(id);
        if (transaction == null)
            return NotFound(ApiResponse<PktTransactionDto>.ErrorResponse("Transaction not found"));

        // Foreman başlamış transactionları düzenleyemez
        if (IsForeman() && transaction.StartOfWork.HasValue)
        {
            return ForbiddenResponse<PktTransactionDto>("Başlamış işlemleri düzenleme yetkiniz yok");
        }

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

        // Foreman başlamış transactionları silemez
        if (IsForeman() && transaction.StartOfWork.HasValue)
        {
            return ForbiddenResponse<bool>("Başlamış işlemleri silme yetkiniz yok");
        }

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

        // ProductionCompleted'a geçerken End zamanını ayarla
        if (statusUpdate.NewStatus == Domain.Enums.TransactionStatus.ProductionCompleted && !transaction.End.HasValue)
        {
            transaction.End = DateTime.UtcNow;
            if (transaction.StartOfWork.HasValue)
            {
                transaction.ActualProductionDuration = transaction.End.Value - transaction.StartOfWork.Value;
            }
        }

        // Washing'e geçerken washing başlangıç zamanını kaydet
        if (statusUpdate.NewStatus == Domain.Enums.TransactionStatus.Washing)
        {
            // Washing başlangıç zamanı - ProductionCompleted'dan geliyor
            if (!transaction.End.HasValue)
            {
                transaction.End = DateTime.UtcNow;
                if (transaction.StartOfWork.HasValue)
                {
                    transaction.ActualProductionDuration = transaction.End.Value - transaction.StartOfWork.Value;
                }
            }
        }

        // Completed'a geçerken (washing olmadan direkt veya washing sonrası)
        if (statusUpdate.NewStatus == Domain.Enums.TransactionStatus.Completed)
        {
            // End zaten ProductionCompleted'da set edildi
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

    [HttpPost("import")]
    public async Task<ActionResult<ApiResponse<ImportResultDto>>> ImportFromExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(ApiResponse<ImportResultDto>.ErrorResponse("Dosya bulunamadı"));

        if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            return BadRequest(ApiResponse<ImportResultDto>.ErrorResponse("Sadece Excel dosyaları yüklenebilir (.xlsx veya .xls)"));

        var result = new ImportResultDto();
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        try
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0]; // İlk sayfa
            var rowCount = worksheet.Dimension?.Rows ?? 0;

            if (rowCount <= 1)
            {
                return BadRequest(ApiResponse<ImportResultDto>.ErrorResponse("Excel dosyası boş veya sadece başlık satırı içeriyor"));
            }

            result.TotalRows = rowCount - 1; // Başlık hariç

            // Tüm reaktörleri, ürünleri ve gecikme nedenlerini memory'ye al (performans için)
            var allReactors = await _reactorRepository.GetAllAsync();
            var allProducts = await _productRepository.GetAllAsync();
            var allDelayReasons = await _delayReasonRepository.GetAllAsync();

            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    // YENİ Excel kolonları
                    // 1. REAKTÖR, 2. Ürün/İşlem, 3. İş Emri No, 4. Lot Numarası, 
                    // 5. Başlangıç Tarih, 6. Başlangıç Saati, 7. Bitiş Tarih, 8. Bitiş Saati,
                    // 9. Yıkama İçin Geçen Süre, 10. Yıkamada kullanılan kostik miktarı (kg),
                    // 11. Reaktör Bekleme / Gecikme Nedeni, 12. Açıklama
                    var reactorName = worksheet.Cells[row, 1].Text?.Trim();
                    var productName = worksheet.Cells[row, 2].Text?.Trim();
                    var workOrderNo = worksheet.Cells[row, 3].Text?.Trim();
                    var lotNo = worksheet.Cells[row, 4].Text?.Trim();
                    var startDate = worksheet.Cells[row, 5].Text?.Trim();
                    var startTime = worksheet.Cells[row, 6].Text?.Trim();
                    var endDate = worksheet.Cells[row, 7].Text?.Trim();
                    var endTime = worksheet.Cells[row, 8].Text?.Trim();
                    var washingDuration = worksheet.Cells[row, 9].Text?.Trim();
                    var causticAmount = worksheet.Cells[row, 10].Text?.Trim();
                    var delayReasonName = worksheet.Cells[row, 11].Text?.Trim();
                    var description = worksheet.Cells[row, 12].Text?.Trim();

                    // Validasyon
                    if (string.IsNullOrEmpty(reactorName))
                    {
                        result.Errors.Add($"Satır {row}: Reaktör adı boş olamaz");
                        result.FailureCount++;
                        continue;
                    }

                    if (string.IsNullOrEmpty(productName))
                    {
                        result.Errors.Add($"Satır {row}: Ürün/İşlem adı boş olamaz");
                        result.FailureCount++;
                        continue;
                    }

                    // Reactor bul
                    var reactor = allReactors.FirstOrDefault(r => r.Name.Equals(reactorName, StringComparison.OrdinalIgnoreCase));
                    if (reactor == null)
                    {
                        result.Errors.Add($"Satır {row}: '{reactorName}' isimli reaktör bulunamadı");
                        result.FailureCount++;
                        continue;
                    }

                    // Product bul (ürün adı veya kodu ile)
                    var product = allProducts.FirstOrDefault(p => 
                        p.ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase) ||
                        p.ProductCode.Equals(productName, StringComparison.OrdinalIgnoreCase));
                    if (product == null)
                    {
                        result.Errors.Add($"Satır {row}: '{productName}' ürünü bulunamadı");
                        result.FailureCount++;
                        continue;
                    }

                    // Delay reason bul (opsiyonel)
                    Guid? delayReasonId = null;
                    if (!string.IsNullOrEmpty(delayReasonName))
                    {
                        var delayReason = allDelayReasons.FirstOrDefault(d => d.Name.Equals(delayReasonName, StringComparison.OrdinalIgnoreCase));
                        if (delayReason != null)
                        {
                            delayReasonId = delayReason.Id;
                        }
                        else
                        {
                            result.Warnings.Add($"Satır {row}: '{delayReasonName}' isimli gecikme nedeni bulunamadı");
                        }
                    }

                    // Tarih parse
                    DateTime? startOfWork = ParseDateTime(startDate, startTime);
                    DateTime? end = ParseDateTime(endDate, endTime);

                    // Süreleri hesapla
                    TimeSpan? actualProductionDuration = null;
                    if (startOfWork.HasValue && end.HasValue)
                    {
                        actualProductionDuration = end.Value - startOfWork.Value;
                    }

                    // Transaction oluştur
                    var transaction = new PktTransaction
                    {
                        Id = Guid.NewGuid(),
                        ReactorId = reactor.Id,
                        ProductId = product.Id,
                        WorkOrderNo = workOrderNo ?? string.Empty,
                        LotNo = lotNo ?? string.Empty,
                        StartOfWork = startOfWork,
                        End = end,
                        ActualProductionDuration = actualProductionDuration,
                        DelayDuration = null, // Gecikme süresi hesaplanmıyor
                        WashingDuration = ParseTimeSpan(washingDuration),
                        CausticAmountKg = ParseDecimal(causticAmount),
                        DelayReasonId = delayReasonId,
                        Description = description,
                        Status = Domain.Enums.TransactionStatus.Completed,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _repository.AddAsync(transaction);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Satır {row}: {ex.Message}");
                    result.FailureCount++;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(ApiResponse<ImportResultDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<ImportResultDto>.ErrorResponse($"Excel işlenirken hata oluştu: {ex.Message}"));
        }
    }

    private DateTime? ParseDateTime(string? dateStr, string? timeStr)
    {
        if (string.IsNullOrEmpty(dateStr)) return null;

        try
        {
            if (DateTime.TryParse(dateStr, out var date))
            {
                if (!string.IsNullOrEmpty(timeStr) && TimeSpan.TryParse(timeStr, out var time))
                {
                    return date.Date + time;
                }
                return date;
            }
        }
        catch { }

        return null;
    }

    private TimeSpan? ParseTimeSpan(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;

        try
        {
            // Saat:dakika formatı (örn: "2:30")
            if (value.Contains(':'))
            {
                if (TimeSpan.TryParse(value, out var ts))
                    return ts;
            }
            // Sadece sayı (saat cinsinden)
            else if (double.TryParse(value, out var hours))
            {
                return TimeSpan.FromHours(hours);
            }
        }
        catch { }

        return null;
    }

    private decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrEmpty(value)) return null;

        if (decimal.TryParse(value, out var result))
            return result;

        return null;
    }
}

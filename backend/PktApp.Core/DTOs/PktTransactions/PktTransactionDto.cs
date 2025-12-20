using PktApp.Domain.Enums;

namespace PktApp.Core.DTOs.PktTransactions;

public class PktTransactionDto
{
    public Guid Id { get; set; }
    public TransactionStatus Status { get; set; }
    public Guid ReactorId { get; set; }
    public string? ReactorName { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string WorkOrderNo { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public DateTime? StartOfWork { get; set; }
    public DateTime? End { get; set; }
    public TimeSpan? ActualProductionDuration { get; set; }
    public TimeSpan? DelayDuration { get; set; }
    public TimeSpan? WashingDuration { get; set; }
    public decimal? CausticAmountKg { get; set; }
    public Guid? DelayReasonId { get; set; }
    public string? DelayReasonName { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreatePktTransactionDto
{
    public Guid ReactorId { get; set; }
    public Guid ProductId { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public decimal? CausticAmountKg { get; set; }
    public Guid? DelayReasonId { get; set; }
    public string? Description { get; set; }
}

public class UpdatePktTransactionDto
{
    public TransactionStatus Status { get; set; }
    public Guid ReactorId { get; set; }
    public Guid ProductId { get; set; }
    public string WorkOrderNo { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public DateTime? StartOfWork { get; set; }
    public DateTime? End { get; set; }
    public TimeSpan? ActualProductionDuration { get; set; }
    public TimeSpan? DelayDuration { get; set; }
    public TimeSpan? WashingDuration { get; set; }
    public decimal? CausticAmountKg { get; set; }
    public Guid? DelayReasonId { get; set; }
    public string? Description { get; set; }
}

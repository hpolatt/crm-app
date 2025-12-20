namespace PKT.Application.DTOs.PktTransactions;

public record PktTransactionDto
{
    public Guid Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public Guid ReactorId { get; init; }
    public string ReactorName { get; init; } = string.Empty;
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string WorkOrderNo { get; init; } = string.Empty;
    public string LotNo { get; init; } = string.Empty;
    public DateTime? StartOfWork { get; init; }
    public DateTime? End { get; init; }
    public TimeSpan? ActualProductionDuration { get; init; }
    public TimeSpan? DelayDuration { get; init; }
    public TimeSpan? WashingDuration { get; init; }
    public decimal? CausticAmountKg { get; init; }
    public Guid? DelayReasonId { get; init; }
    public string? DelayReasonName { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreatePktTransactionDto
{
    public string Status { get; init; } = string.Empty;
    public Guid ReactorId { get; init; }
    public Guid ProductId { get; init; }
    public string WorkOrderNo { get; init; } = string.Empty;
    public string LotNo { get; init; } = string.Empty;
    public DateTime? StartOfWork { get; init; }
    public DateTime? End { get; init; }
    public TimeSpan? ActualProductionDuration { get; init; }
    public TimeSpan? DelayDuration { get; init; }
    public TimeSpan? WashingDuration { get; init; }
    public decimal? CausticAmountKg { get; init; }
    public Guid? DelayReasonId { get; init; }
    public string? Description { get; init; }
}

public record UpdatePktTransactionDto
{
    public string Status { get; init; } = string.Empty;
    public Guid ReactorId { get; init; }
    public Guid ProductId { get; init; }
    public string WorkOrderNo { get; init; } = string.Empty;
    public string LotNo { get; init; } = string.Empty;
    public DateTime? StartOfWork { get; init; }
    public DateTime? End { get; init; }
    public TimeSpan? ActualProductionDuration { get; init; }
    public TimeSpan? DelayDuration { get; init; }
    public TimeSpan? WashingDuration { get; init; }
    public decimal? CausticAmountKg { get; init; }
    public Guid? DelayReasonId { get; init; }
    public string? Description { get; init; }
}

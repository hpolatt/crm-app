namespace PktApp.Domain.Entities;

public class PktTransaction : BaseEntity
{
    public string Status { get; set; } = string.Empty;
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
    
    // Navigation properties
    public Reactor Reactor { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public DelayReason? DelayReason { get; set; }
}

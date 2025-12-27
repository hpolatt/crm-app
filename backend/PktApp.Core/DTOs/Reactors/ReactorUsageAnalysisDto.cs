namespace PktApp.Core.DTOs.Reactors;

public class ReactorUsageAnalysisDto
{
    public Guid ReactorId { get; set; }
    public string ReactorName { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public TimeSpan TotalProductionDuration { get; set; }
    public TimeSpan TotalWashingDuration { get; set; }
    public TimeSpan TotalDelayDuration { get; set; }
}

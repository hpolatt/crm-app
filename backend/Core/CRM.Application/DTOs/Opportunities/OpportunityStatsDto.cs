namespace CRM.Application.DTOs.Opportunities;

public class OpportunityStatsDto
{
    public int TotalCount { get; set; }
    public decimal TotalValue { get; set; }
    public int WonCount { get; set; }
    public decimal WonValue { get; set; }
    public int LostCount { get; set; }
    public decimal LostValue { get; set; }
    public int OpenCount { get; set; }
    public decimal OpenValue { get; set; }
    public double WinRate { get; set; }
    public Dictionary<string, int> ByStage { get; set; } = new();
    public Dictionary<string, decimal> ValueByStage { get; set; } = new();
}

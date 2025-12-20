namespace PKT.Domain.Entities;

public class Product : BaseEntity
{
    public string SBU { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal MinProductionQuantity { get; set; }
    public decimal MaxProductionQuantity { get; set; }
    public int ProductionDurationHours { get; set; }
    public string? Notes { get; set; }
    
    // Navigation property
    public ICollection<PktTransaction> PktTransactions { get; set; } = new List<PktTransaction>();
}

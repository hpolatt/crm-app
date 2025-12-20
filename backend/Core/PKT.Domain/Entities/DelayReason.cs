namespace PKT.Domain.Entities;

public class DelayReason : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<PktTransaction> PktTransactions { get; set; } = new List<PktTransaction>();
}

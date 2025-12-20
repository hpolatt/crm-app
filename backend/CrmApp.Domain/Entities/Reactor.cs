namespace PKTApp.Domain.Entities;

public class Reactor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<PktTransaction> PktTransactions { get; set; } = new List<PktTransaction>();
}

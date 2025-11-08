namespace CrmApp.Domain.Entities;

public class DealStage : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool IsDefault { get; set; }
    
    // Navigation properties
    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}

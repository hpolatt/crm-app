namespace CrmApp.Domain.Entities;

public class Note : BaseEntity
{
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    
    // Navigation properties
    public Company? Company { get; set; }
    public Contact? Contact { get; set; }
    public Lead? Lead { get; set; }
    public Opportunity? Opportunity { get; set; }
}

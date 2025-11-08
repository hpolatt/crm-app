namespace CRM.Domain.Entities;

public class Activity : BaseEntity
{
    public string Type { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "planned";
    public string Priority { get; set; } = "medium";
    public DateTime? DueDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? AssignedUserId { get; set; }
    
    // Navigation properties
    public Company? Company { get; set; }
    public Contact? Contact { get; set; }
    public Lead? Lead { get; set; }
    public Opportunity? Opportunity { get; set; }
    public User? AssignedUser { get; set; }
}

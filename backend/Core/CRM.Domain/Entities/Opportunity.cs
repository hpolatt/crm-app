namespace CRM.Domain.Entities;

public class Opportunity : BaseEntity
{
    public Guid? LeadId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Stage { get; set; } = "prospecting";
    public decimal Value { get; set; }
    public int? Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Lead? Lead { get; set; }
    public Company? Company { get; set; }
    public Contact? Contact { get; set; }
    public User? AssignedUser { get; set; }
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}

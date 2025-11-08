namespace CRM.Domain.Entities;

public class Lead : BaseEntity
{
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string Status { get; set; } = "new";
    public decimal? Value { get; set; }
    public int? Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? Notes { get; set; }
    
    // Navigation properties
    public Company? Company { get; set; }
    public Contact? Contact { get; set; }
    public User? AssignedUser { get; set; }
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}

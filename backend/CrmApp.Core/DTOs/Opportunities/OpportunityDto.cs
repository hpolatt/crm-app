namespace CrmApp.Core.DTOs.Opportunities;

public class OpportunityDto
{
    public Guid Id { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid? ContactId { get; set; }
    public string? ContactName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Stage { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public int? Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public DateTime? ActualCloseDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? AssignedUserIdName { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

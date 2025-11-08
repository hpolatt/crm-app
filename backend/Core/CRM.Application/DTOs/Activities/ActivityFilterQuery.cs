namespace CRM.Application.DTOs.Activities;

public class ActivityFilterQuery
{
    public string? Type { get; set; }
    public string? Status { get; set; }
    public Guid? AssignedUserId { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? DueDateFrom { get; set; }
    public DateTime? DueDateTo { get; set; }
    public string? Priority { get; set; }
}

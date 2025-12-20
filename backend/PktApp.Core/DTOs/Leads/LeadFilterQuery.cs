namespace PktApp.Core.DTOs.Leads;

public class LeadFilterQuery
{
    public string? Title { get; set; }
    public string? Status { get; set; }
    public string? Source { get; set; }
    public bool? IsActive { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}

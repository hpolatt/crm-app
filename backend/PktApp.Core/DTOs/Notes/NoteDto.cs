namespace PktApp.Core.DTOs.Notes;

public class NoteDto
{
    public Guid Id { get; set; }
    public Guid? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public Guid? ContactId { get; set; }
    public string? ContactName { get; set; }
    public Guid? LeadId { get; set; }
    public string? LeadName { get; set; }
    public Guid? OpportunityId { get; set; }
    public string? OpportunityTitle { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public string? CreatedByName { get; set; }
    public bool IsDeleted { get; set; }
}

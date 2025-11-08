namespace CrmApp.Core.DTOs.Notes;

public class NoteFilterQuery
{
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid? LeadId { get; set; }
    public Guid? OpportunityId { get; set; }
    public Guid? CreatedBy { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}

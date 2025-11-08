namespace CrmApp.Core.DTOs.Leads;

public class CreateLeadDto
{
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Source { get; set; }
    public string? Status { get; set; }
    public decimal? Value { get; set; }
    public int? Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? Notes { get; set; }
}

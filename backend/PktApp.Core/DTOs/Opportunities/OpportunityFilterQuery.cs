namespace PktApp.Core.DTOs.Opportunities;

public class OpportunityFilterQuery
{
    public string? Title { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? ContactId { get; set; }
    public string? Stage { get; set; }
    public decimal? AmountMin { get; set; }
    public decimal? AmountMax { get; set; }
    public int? ProbabilityMin { get; set; }
    public int? ProbabilityMax { get; set; }
    public DateTime? ExpectedCloseDateFrom { get; set; }
    public DateTime? ExpectedCloseDateTo { get; set; }
    public bool? IsActive { get; set; }
    public Guid? AssignedUserId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}

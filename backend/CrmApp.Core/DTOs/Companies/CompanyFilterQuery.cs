namespace CrmApp.Core.DTOs.Companies;

public class CompanyFilterQuery
{
    public string? Name { get; set; }
    public string? Industry { get; set; }
    public string? Source { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SearchTerm { get; set; }
}

namespace CrmApp.Core.DTOs.Contacts;

public class ContactFilterQuery
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public Guid? CompanyId { get; set; }
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}

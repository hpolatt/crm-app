namespace CRM.Domain.Entities;

public class Contact : BaseEntity
{
    public Guid? CompanyId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Notes { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Company? Company { get; set; }
    public ICollection<Lead> Leads { get; set; } = new List<Lead>();
    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}

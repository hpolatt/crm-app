using System.ComponentModel.DataAnnotations;

namespace CrmApp.Core.DTOs.Companies;

public class CreateCompanyDto
{
    [Required(ErrorMessage = "Şirket adı zorunludur")]
    [StringLength(255, ErrorMessage = "Şirket adı 255 karakteri aşamaz")]
    public string Name { get; set; } = string.Empty;
    
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    
    [Required(ErrorMessage = "Email zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    [StringLength(255, ErrorMessage = "Email 255 karakteri aşamaz")]
    public string Email { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? Source { get; set; }
    public int? EmployeeCount { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Notes { get; set; }
}

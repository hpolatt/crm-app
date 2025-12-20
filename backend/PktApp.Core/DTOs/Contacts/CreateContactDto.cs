using System.ComponentModel.DataAnnotations;

namespace PktApp.Core.DTOs.Contacts;

public class CreateContactDto
{
    public Guid? CompanyId { get; set; }
    
    [Required(ErrorMessage = "First name is required")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Last name is required")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;
    
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
    public string? Email { get; set; }
    
    [Phone(ErrorMessage = "Invalid phone format")]
    [StringLength(20, ErrorMessage = "Phone cannot exceed 20 characters")]
    public string? Phone { get; set; }
    
    [Phone(ErrorMessage = "Invalid mobile format")]
    [StringLength(20, ErrorMessage = "Mobile cannot exceed 20 characters")]
    public string? Mobile { get; set; }
    
    [StringLength(100, ErrorMessage = "Position cannot exceed 100 characters")]
    public string? Position { get; set; }
    
    [StringLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
    public string? Department { get; set; }
    
    public string? Address { get; set; }
    
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string? City { get; set; }
    
    [StringLength(100, ErrorMessage = "Country cannot exceed 100 characters")]
    public string? Country { get; set; }
    
    [StringLength(20, ErrorMessage = "Postal code cannot exceed 20 characters")]
    public string? PostalCode { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsPrimary { get; set; }
    
    public bool IsActive { get; set; } = true;
}

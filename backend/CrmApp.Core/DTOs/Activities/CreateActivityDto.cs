using System.ComponentModel.DataAnnotations;

namespace CrmApp.Core.DTOs.Activities;

public class CreateActivityDto
{
    [Required(ErrorMessage = "Type is required")]
    [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters")]
    public string Type { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Subject is required")]
    [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
    public string Subject { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Status is required")]
    [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
    public string Status { get; set; } = "planned";
    
    [Required(ErrorMessage = "Priority is required")]
    [StringLength(50, ErrorMessage = "Priority cannot exceed 50 characters")]
    public string Priority { get; set; } = "medium";
    
    public DateTime? DueDate { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public Guid? ContactId { get; set; }
    
    public Guid? LeadId { get; set; }
    
    public Guid? OpportunityId { get; set; }
    
    public Guid? AssignedUserId { get; set; }
    
    public bool IsActive { get; set; } = true;
}

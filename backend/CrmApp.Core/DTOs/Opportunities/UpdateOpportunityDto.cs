using System.ComponentModel.DataAnnotations;

namespace CrmApp.Core.DTOs.Opportunities;

public class UpdateOpportunityDto
{
    public Guid? LeadId { get; set; }
    
    public Guid? CompanyId { get; set; }
    
    public Guid? ContactId { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    [Required(ErrorMessage = "Stage is required")]
    [StringLength(50, ErrorMessage = "Stage cannot exceed 50 characters")]
    public string Stage { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue, ErrorMessage = "Value must be a positive number")]
    public decimal Value { get; set; }
    
    [Range(0, 100, ErrorMessage = "Probability must be between 0 and 100")]
    public int? Probability { get; set; }
    
    public DateTime? ExpectedCloseDate { get; set; }
    
    public DateTime? ActualCloseDate { get; set; }
    
    public Guid? AssignedUserId { get; set; }
    
    public string? Notes { get; set; }
    
    public bool IsActive { get; set; }
}
    
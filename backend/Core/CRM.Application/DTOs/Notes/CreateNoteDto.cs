using System.ComponentModel.DataAnnotations;

namespace CRM.Application.DTOs.Notes;

public class CreateNoteDto
{
    public Guid? CompanyId { get; set; }
    
    public Guid? ContactId { get; set; }
    
    public Guid? LeadId { get; set; }
    
    public Guid? OpportunityId { get; set; }
    
    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
    public string Content { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; }
    
    public bool IsActive { get; set; } = true;
}

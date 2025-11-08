using System.ComponentModel.DataAnnotations;

namespace CrmApp.Core.DTOs.Notes;

public class UpdateNoteDto
{
    [Required(ErrorMessage = "Content is required")]
    [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
    public string Content { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; }
    
    public bool IsActive { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace CrmApp.Core.DTOs.DealStages;

public class UpdateDealStageDto
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    [Range(0, int.MaxValue, ErrorMessage = "Order must be a positive number")]
    public int Order { get; set; }
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    [StringLength(20, ErrorMessage = "Color cannot exceed 20 characters")]
    public string? Color { get; set; }
    
    public bool IsDefault { get; set; }
    
    public bool IsActive { get; set; }
}

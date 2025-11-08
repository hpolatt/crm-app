using System.ComponentModel.DataAnnotations;

namespace CRM.Application.DTOs.Settings;

public class UpdateSystemSettingDto
{
    [Required(ErrorMessage = "Key is required")]
    [StringLength(100, ErrorMessage = "Key cannot exceed 100 characters")]
    public string Key { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Value is required")]
    public string Value { get; set; } = string.Empty;
    
    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    [StringLength(50, ErrorMessage = "DataType cannot exceed 50 characters")]
    public string DataType { get; set; } = "string";
    
    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }
    
    public bool IsPublic { get; set; }
}

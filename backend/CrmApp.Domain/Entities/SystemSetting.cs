namespace CrmApp.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DataType { get; set; } = "string"; // string, int, bool, json
    public string? Category { get; set; }
    public bool IsPublic { get; set; } // Can be accessed by non-admin users
}

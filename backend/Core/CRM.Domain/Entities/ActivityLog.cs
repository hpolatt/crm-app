namespace CRM.Domain.Entities;

public class ActivityLog : BaseEntity
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout, etc.
    public string EntityType { get; set; } = string.Empty; // Company, Contact, Lead, etc.
    public Guid? EntityId { get; set; }
    public string? EntityName { get; set; }
    public string? Description { get; set; } // JSON string with additional details
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    // Navigation properties
    public User? User { get; set; }
}

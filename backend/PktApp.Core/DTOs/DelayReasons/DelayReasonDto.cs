namespace PktApp.Core.DTOs.DelayReasons;

public class DelayReasonDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateDelayReasonDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateDelayReasonDto
{
    public string Name { get; set; } = string.Empty;
}

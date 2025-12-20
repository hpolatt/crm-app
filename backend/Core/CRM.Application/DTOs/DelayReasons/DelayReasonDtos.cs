namespace PKT.Application.DTOs.DelayReasons;

public record DelayReasonDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateDelayReasonDto
{
    public string Name { get; init; } = string.Empty;
}

public record UpdateDelayReasonDto
{
    public string Name { get; init; } = string.Empty;
}

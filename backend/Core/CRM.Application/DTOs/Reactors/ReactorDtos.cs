namespace PKT.Application.DTOs.Reactors;

public record ReactorDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateReactorDto
{
    public string Name { get; init; } = string.Empty;
}

public record UpdateReactorDto
{
    public string Name { get; init; } = string.Empty;
}

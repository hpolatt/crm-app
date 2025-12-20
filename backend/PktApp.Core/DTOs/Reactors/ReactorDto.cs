namespace PktApp.Core.DTOs.Reactors;

public class ReactorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateReactorDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateReactorDto
{
    public string Name { get; set; } = string.Empty;
}

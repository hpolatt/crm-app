namespace PKT.Application.DTOs.Products;

public record ProductDto
{
    public Guid Id { get; init; }
    public string SBU { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal MinProductionQuantity { get; init; }
    public decimal MaxProductionQuantity { get; init; }
    public int ProductionDurationMinutes { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public record CreateProductDto
{
    public string SBU { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal MinProductionQuantity { get; init; }
    public decimal MaxProductionQuantity { get; init; }
    public int ProductionDurationMinutes { get; init; }
    public string? Notes { get; init; }
}

public record UpdateProductDto
{
    public string SBU { get; init; } = string.Empty;
    public string ProductCode { get; init; } = string.Empty;
    public string ProductName { get; init; } = string.Empty;
    public decimal MinProductionQuantity { get; init; }
    public decimal MaxProductionQuantity { get; init; }
    public int ProductionDurationMinutes { get; init; }
    public string? Notes { get; init; }
}

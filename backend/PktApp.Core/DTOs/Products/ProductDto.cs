namespace PktApp.Core.DTOs.Products;

public class ProductDto
{
    public Guid Id { get; set; }
    public string SBU { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal MinProductionQuantity { get; set; }
    public decimal MaxProductionQuantity { get; set; }
    public int ProductionDurationHours { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateProductDto
{
    public string SBU { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal MinProductionQuantity { get; set; }
    public decimal MaxProductionQuantity { get; set; }
    public int ProductionDurationHours { get; set; }
    public string? Notes { get; set; }
}

public class UpdateProductDto
{
    public string SBU { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal MinProductionQuantity { get; set; }
    public decimal MaxProductionQuantity { get; set; }
    public int ProductionDurationHours { get; set; }
    public string? Notes { get; set; }
}

using Microsoft.AspNetCore.Mvc;
using PktApp.Core.DTOs.Common;
using PktApp.Core.DTOs.Products;
using PktApp.Core.Interfaces;
using PktApp.Domain.Entities;

namespace PktApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly IRepository<Product> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IRepository<Product> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
    {
        var products = await _repository.GetAllAsync();
        var dtos = products.Select(p => new ProductDto
        {
            Id = p.Id,
            SBU = p.SBU,
            ProductCode = p.ProductCode,
            ProductName = p.ProductName,
            MinProductionQuantity = p.MinProductionQuantity,
            MaxProductionQuantity = p.MaxProductionQuantity,
            ProductionDurationHours = p.ProductionDurationHours,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
        return Ok(ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(dtos));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<ProductDto>.ErrorResponse("Product not found"));

        var dto = new ProductDto
        {
            Id = product.Id,
            SBU = product.SBU,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            MinProductionQuantity = product.MinProductionQuantity,
            MaxProductionQuantity = product.MaxProductionQuantity,
            ProductionDurationHours = product.ProductionDurationHours,
            Notes = product.Notes,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
        return Ok(ApiResponse<ProductDto>.SuccessResponse(dto));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Create(CreateProductDto createDto)
    {
        var product = new Product
        {
            SBU = createDto.SBU,
            ProductCode = createDto.ProductCode,
            ProductName = createDto.ProductName,
            MinProductionQuantity = createDto.MinProductionQuantity,
            MaxProductionQuantity = createDto.MaxProductionQuantity,
            ProductionDurationHours = createDto.ProductionDurationHours,
            Notes = createDto.Notes
        };

        await _repository.AddAsync(product);
        await _unitOfWork.CommitAsync();

        var dto = new ProductDto
        {
            Id = product.Id,
            SBU = product.SBU,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            MinProductionQuantity = product.MinProductionQuantity,
            MaxProductionQuantity = product.MaxProductionQuantity,
            ProductionDurationHours = product.ProductionDurationHours,
            Notes = product.Notes,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, ApiResponse<ProductDto>.SuccessResponse(dto));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> Update(Guid id, UpdateProductDto updateDto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<ProductDto>.ErrorResponse("Product not found"));

        product.SBU = updateDto.SBU;
        product.ProductCode = updateDto.ProductCode;
        product.ProductName = updateDto.ProductName;
        product.MinProductionQuantity = updateDto.MinProductionQuantity;
        product.MaxProductionQuantity = updateDto.MaxProductionQuantity;
        product.ProductionDurationHours = updateDto.ProductionDurationHours;
        product.Notes = updateDto.Notes;

        _repository.Update(product);
        await _unitOfWork.CommitAsync();

        var dto = new ProductDto
        {
            Id = product.Id,
            SBU = product.SBU,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            MinProductionQuantity = product.MinProductionQuantity,
            MaxProductionQuantity = product.MaxProductionQuantity,
            ProductionDurationHours = product.ProductionDurationHours,
            Notes = product.Notes,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return Ok(ApiResponse<ProductDto>.SuccessResponse(dto));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            return NotFound(ApiResponse<bool>.ErrorResponse("Product not found"));

        _repository.Remove(product);
        await _unitOfWork.CommitAsync();

        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}

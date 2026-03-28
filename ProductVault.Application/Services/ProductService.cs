// ProductVault.Application/Services/ProductService.cs
using Microsoft.Extensions.Logging;
using ProductVault.Application.DTOs;
using ProductVault.Domain.Entities;
using ProductVault.Domain.Interfaces;

namespace ProductVault.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repo;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IProductRepository repo, ILogger<ProductService> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all products");
        var products = await _repo.GetAllAsync();
        _logger.LogInformation("Returned {Count} products", products.Count);
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching product with ID {ProductId}", id);
        var product = await _repo.GetByIdAsync(id);
        if (product is null)
        {
            _logger.LogWarning("Product with ID {ProductId} was not found", id);
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
        }
        return ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        _logger.LogInformation("Creating product: {Name}", dto.Name);
        var product = new Product
        {
            Name     = dto.Name,
            Price    = dto.Price,
            Category = dto.Category
        };
        var created = await _repo.CreateAsync(product);
        _logger.LogInformation("Product created with ID {ProductId}", created.Id);
        return ToDto(created);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        _logger.LogInformation("Updating product with ID {ProductId}", id);
        var updated = new Product
        {
            Name     = dto.Name,
            Price    = dto.Price,
            Category = dto.Category
        };
        var result = await _repo.UpdateAsync(id, updated);
        if (result is null)
        {
            _logger.LogWarning("Update failed — product with ID {ProductId} not found", id);
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
        }
        return ToDto(result);
    }

    public async Task DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product with ID {ProductId}", id);
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Delete failed — product with ID {ProductId} not found", id);
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
        }
    }

    private static ProductDto ToDto(Product p) =>
        new(p.Id, p.Name, p.Price, p.Category, p.CreatedAt);
}
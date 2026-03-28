// ProductVault.Application/Services/ProductService.cs
using ProductVault.Application.DTOs;
using ProductVault.Domain.Entities;
using ProductVault.Domain.Interfaces;

namespace ProductVault.Application.Services;

public class ProductService
{
    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _repo.GetAllAsync();
        return products.Select(ToDto).ToList();
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _repo.GetByIdAsync(id);
        if (product is null)
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
        return ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name     = dto.Name,
            Price    = dto.Price,
            Category = dto.Category
        };
        var created = await _repo.CreateAsync(product);
        return ToDto(created);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var updated = new Product
        {
            Name     = dto.Name,
            Price    = dto.Price,
            Category = dto.Category
        };
        var result = await _repo.UpdateAsync(id, updated);
        if (result is null)
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
        return ToDto(result);
    }

    public async Task DeleteAsync(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Product with ID {id} was not found.");
    }

    // Private helper — converts Entity → DTO
    private static ProductDto ToDto(Product p) =>
        new(p.Id, p.Name, p.Price, p.Category, p.CreatedAt);
}
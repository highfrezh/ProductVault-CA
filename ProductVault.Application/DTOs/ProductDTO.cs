// ProductVault.Application/DTOs/ProductDto.cs
namespace ProductVault.Application.DTOs;

public record ProductDto(int Id, string Name, decimal Price, string Category, DateTime CreatedAt);

public record CreateProductDto(string Name, decimal Price, string Category);

public record UpdateProductDto(string Name, decimal Price, string Category);
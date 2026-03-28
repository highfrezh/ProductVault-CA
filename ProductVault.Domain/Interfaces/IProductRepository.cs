// ProductVault.Domain/Interfaces/IProductRepository.cs
using ProductVault.Domain.Entities;

namespace ProductVault.Domain.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(int id, Product updated);
    Task<bool> DeleteAsync(int id);
}
// ProductVault.Infrastructure/Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProductVault.Domain.Entities;

namespace ProductVault.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
}
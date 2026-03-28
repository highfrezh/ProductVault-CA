// ProductVault.Tests/Services/ProductServiceTests.cs
using Microsoft.Extensions.Logging;
using Moq;
using ProductVault.Application.DTOs;
using ProductVault.Application.Services;
using ProductVault.Domain.Entities;
using ProductVault.Domain.Interfaces;

namespace ProductVault.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly Mock<ILogger<ProductService>> _mockLogger;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo   = new Mock<IProductRepository>();
        _mockLogger = new Mock<ILogger<ProductService>>();
        _service    = new ProductService(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        var fakeProducts = new List<Product>
        {
            new() { Id = 1, Name = "Laptop",  Price = 999.99m, Category = "Electronics" },
            new() { Id = 2, Name = "Monitor", Price = 299.99m, Category = "Electronics" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeProducts);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Laptop", result[0].Name);
    }

    [Fact]
    public async Task GetByIdAsync_ProductExists_ReturnsProduct()
    {
        var fakeProduct = new Product
        {
            Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics"
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeProduct);

        var result = await _service.GetByIdAsync(1);

        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_ValidInput_ReturnsCreatedProduct()
    {
        var dto = new CreateProductDto("Keyboard", 79.99m, "Electronics");
        var savedProduct = new Product
        {
            Id = 3, Name = "Keyboard", Price = 79.99m, Category = "Electronics"
        };
        _mockRepo
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(savedProduct);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(3, result.Id);
        Assert.Equal("Keyboard", result.Name);
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ProductNotFound_ThrowsKeyNotFoundException()
    {
        _mockRepo.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.DeleteAsync(99));
    }

    [Theory]
    [InlineData("Laptop",   999.99, "Electronics")]
    [InlineData("Desk",     199.99, "Furniture")]
    [InlineData("Notebook", 4.99,   "Stationery")]
    public async Task CreateAsync_VariousProducts_AllSucceed(
        string name, decimal price, string category)
    {
        var dto   = new CreateProductDto(name, price, category);
        var saved = new Product { Id = 1, Name = name, Price = price, Category = category };

        _mockRepo
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(saved);

        var result = await _service.CreateAsync(dto);

        Assert.Equal(name,     result.Name);
        Assert.Equal(price,    result.Price);
        Assert.Equal(category, result.Category);
    }
}
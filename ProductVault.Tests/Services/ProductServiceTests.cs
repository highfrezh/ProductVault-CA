// ProductVault.Tests/Services/ProductServiceTests.cs
using Moq;
using ProductVault.Application.DTOs;
using ProductVault.Application.Services;
using ProductVault.Domain.Entities;
using ProductVault.Domain.Interfaces;

namespace ProductVault.Tests.Services;

public class ProductServiceTests
{
    // These are shared across all tests in this class
    private readonly Mock<IProductRepository> _mockRepo;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepo = new Mock<IProductRepository>();
        _service  = new ProductService(_mockRepo.Object);
    }

    // ✅ Test 1 — GetAll returns correct number of products
    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange — fake data the repo will return
        var fakeProducts = new List<Product>
        {
            new() { Id = 1, Name = "Laptop",  Price = 999.99m, Category = "Electronics" },
            new() { Id = 2, Name = "Monitor", Price = 299.99m, Category = "Electronics" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fakeProducts);

        // Act — call the real service method
        var result = await _service.GetAllAsync();

        // Assert — verify the output
        Assert.Equal(2, result.Count);
        Assert.Equal("Laptop", result[0].Name);
        Assert.Equal("Monitor", result[1].Name);
    }

    // ✅ Test 2 — GetById returns correct product when it exists
    [Fact]
    public async Task GetByIdAsync_ProductExists_ReturnsProduct()
    {
        // Arrange
        var fakeProduct = new Product
        {
            Id = 1, Name = "Laptop", Price = 999.99m, Category = "Electronics"
        };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeProduct);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Laptop", result.Name);
        Assert.Equal(999.99m, result.Price);
    }

    // ✅ Test 3 — GetById throws when product does NOT exist
    [Fact]
    public async Task GetByIdAsync_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange — repo returns null (product doesn't exist)
        _mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Product?)null);

        // Act & Assert — service must throw
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.GetByIdAsync(99));
    }

    // ✅ Test 4 — Create returns the newly created product
    [Fact]
    public async Task CreateAsync_ValidInput_ReturnsCreatedProduct()
    {
        // Arrange
        var dto = new CreateProductDto("Keyboard", 79.99m, "Electronics");
        var savedProduct = new Product
        {
            Id = 3, Name = "Keyboard", Price = 79.99m, Category = "Electronics"
        };
        _mockRepo
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(savedProduct);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(3, result.Id);
        Assert.Equal("Keyboard", result.Name);

        // Verify CreateAsync was called exactly once
        _mockRepo.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    // ✅ Test 5 — Delete throws when product does NOT exist
    [Fact]
    public async Task DeleteAsync_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange — repo returns false meaning nothing was deleted
        _mockRepo.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.DeleteAsync(99));
    }

    // ✅ Test 6 — Multiple create scenarios using Theory
    [Theory]
    [InlineData("Laptop",   999.99, "Electronics")]
    [InlineData("Desk",     199.99, "Furniture")]
    [InlineData("Notebook", 4.99,   "Stationery")]
    public async Task CreateAsync_VariousProducts_AllSucceed(
        string name, decimal price, string category)
    {
        // Arrange
        var dto = new CreateProductDto(name, price, category);
        var saved = new Product { Id = 1, Name = name, Price = price, Category = category };

        _mockRepo
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(saved);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(name, result.Name);
        Assert.Equal(price, result.Price);
        Assert.Equal(category, result.Category);
    }
}
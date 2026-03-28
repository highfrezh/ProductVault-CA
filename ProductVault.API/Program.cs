using Microsoft.EntityFrameworkCore;
using ProductVault.Application.Services;
using ProductVault.Domain.Interfaces;
using ProductVault.Infrastructure.Data;
using ProductVault.Infrastructure.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();  // .NET 9 built-in, no extra package needed

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();   // UI lives at /scalar/v1

app.UseAuthorization();
app.MapControllers();
app.Run();
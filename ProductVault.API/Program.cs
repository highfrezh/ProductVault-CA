using Microsoft.EntityFrameworkCore;
using ProductVault.Application.Services;
using ProductVault.Domain.Interfaces;
using ProductVault.Infrastructure.Data;
using ProductVault.Infrastructure.Repositories;
using ProductVault.API.Middleware;
using Scalar.AspNetCore;
using Serilog;

// ── Serilog Setup ────────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/productvault.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Use Serilog instead of default logger
builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Dependency Injection
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// ── Middleware — order matters! ───────────────────────────
app.UseMiddleware<GlobalExceptionMiddleware>(); // must be FIRST
app.MapOpenApi();
app.MapScalarApiReference();
app.UseAuthorization();
app.MapControllers();

// Auto-apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
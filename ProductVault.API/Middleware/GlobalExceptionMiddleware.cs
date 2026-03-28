// ProductVault.API/Middleware/GlobalExceptionMiddleware.cs
using System.Net;
using System.Text.Json;

namespace ProductVault.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context); // pass request down the pipeline
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception caught: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        // Map exception types to HTTP status codes
        var (statusCode, message) = ex switch
        {
            KeyNotFoundException        => (HttpStatusCode.NotFound,           ex.Message),
            ArgumentException           => (HttpStatusCode.BadRequest,         ex.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,       "Unauthorized."),
            _                           => (HttpStatusCode.InternalServerError,"An unexpected error occurred.")
        };

        var response = new
        {
            statusCode = (int)statusCode,
            error      = message,
            timestamp  = DateTime.UtcNow
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode  = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
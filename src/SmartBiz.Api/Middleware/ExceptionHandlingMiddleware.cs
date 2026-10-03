using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SmartBiz.Application.Exceptions;

namespace SmartBiz.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Log severity based on exception type
        switch (exception)
        {
            case AuthException auth:
                _logger.LogWarning(auth, "Authentication error: {Message}", auth.Message);
                break;
            case ValidationException validation:
                _logger.LogInformation("Validation failed: {Count} error(s)", validation.Errors.Count);
                break;
            default:
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                break;
        }

        var (statusCode, problemDetails) = MapException(context, exception);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var json = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(json);
    }

    private (int StatusCode, ProblemDetails Problem) MapException(HttpContext context, Exception exception)
    {
        switch (exception)
        {
            case AuthException auth:
                return (auth.StatusCode, new ProblemDetails
                {
                    Status = auth.StatusCode,
                    Title = "Authentication failed",
                    Detail = auth.Message,
                    Instance = context.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
                });

            case ValidationException validation:
                var problem = new ValidationProblemDetails(
                    validation.Errors.ToDictionary(k => k.Key, v => v.Value))
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Validation failed",
                    Instance = context.Request.Path,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                };
                return (StatusCodes.Status400BadRequest, problem);

            default:
                var (code, title) = exception switch
                {
                    UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
                    KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                    ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request"),
                    InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
                    _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
                };

                var details = new ProblemDetails
                {
                    Status = code,
                    Title = title,
                    Instance = context.Request.Path
                };

                if (_env.IsDevelopment())
                {
                    details.Detail = exception.ToString();
                }

                return (code, details);
        }
    }
}
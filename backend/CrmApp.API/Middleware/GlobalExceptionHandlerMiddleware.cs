using System.Diagnostics;
using System.Net;
using System.Security.Claims;
using CrmApp.Core.DTOs;
using CrmApp.Core.DTOs.Logging;
using CrmApp.Core.Interfaces;

namespace CrmApp.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IElasticsearchService elasticsearchService)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex, elasticsearchService);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context, 
        Exception exception,
        IElasticsearchService elasticsearchService)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var requestId = context.TraceIdentifier;

        // Log to Elasticsearch
        var log = new RequestLogDto
        {
            Timestamp = DateTime.UtcNow,
            RequestId = requestId,
            Method = context.Request.Method,
            Path = context.Request.Path,
            QueryString = context.Request.QueryString.ToString(),
            StatusCode = context.Response.StatusCode,
            DurationMs = 0, // Will be calculated in RequestLoggingMiddleware
            UserId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            UserEmail = context.User?.FindFirst(ClaimTypes.Email)?.Value,
            IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            UserAgent = context.Request.Headers["User-Agent"].ToString(),
            ErrorMessage = exception.Message,
            StackTrace = exception.StackTrace,
            CustomFields = new Dictionary<string, object>
            {
                ["ExceptionType"] = exception.GetType().Name,
                ["InnerException"] = exception.InnerException?.Message ?? string.Empty,
                ["IsError"] = true
            }
        };

        // Log to Elasticsearch asynchronously
        _ = Task.Run(async () =>
        {
            try
            {
                await elasticsearchService.IndexRequestLogAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log exception to Elasticsearch");
            }
        });

        // Return error response
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "An unexpected error occurred",
            Errors = new List<string> { exception.Message }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}

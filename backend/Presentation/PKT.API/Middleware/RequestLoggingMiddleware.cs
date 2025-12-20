using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using PKT.Application.DTOs.Logging;
using PKT.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PKT.API.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IElasticsearchService elasticsearchService)
    {
        var requestId = Guid.NewGuid().ToString();
        context.TraceIdentifier = requestId;

        var stopwatch = Stopwatch.StartNew();
        
        // Capture request body
        var requestBody = await ReadRequestBodyAsync(context.Request);
        
        // Store original response body stream
        var originalResponseBodyStream = context.Response.Body;
        
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        Exception? exception = null;
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            exception = ex;
            throw;
        }
        finally
        {
            stopwatch.Stop();

            // Read response body
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // Copy response to original stream
            await responseBodyStream.CopyToAsync(originalResponseBodyStream);
            context.Response.Body = originalResponseBodyStream;

            // Create log entry
            var log = new RequestLogDto
            {
                Timestamp = DateTime.UtcNow,
                RequestId = requestId,
                Method = context.Request.Method,
                Path = context.Request.Path,
                QueryString = context.Request.QueryString.ToString(),
                StatusCode = context.Response.StatusCode,
                DurationMs = stopwatch.ElapsedMilliseconds,
                UserId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                UserEmail = context.User?.FindFirst(ClaimTypes.Email)?.Value,
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                RequestHeaders = context.Request.Headers
                    .Where(h => !h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(h => h.Key, h => h.Value.ToString()),
                RequestBody = ShouldLogBody(context.Request) ? requestBody : null,
                ResponseBody = ShouldLogBody(context.Request) && context.Response.StatusCode >= 400 
                    ? responseBody 
                    : null,
                ErrorMessage = exception?.Message,
                StackTrace = exception?.StackTrace
            };

            // Add custom fields
            if (context.Response.StatusCode >= 400)
            {
                log.CustomFields["IsError"] = true;
            }

            // Log to Elasticsearch asynchronously (fire and forget)
            _ = Task.Run(async () =>
            {
                try
                {
                    await elasticsearchService.IndexRequestLogAsync(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to log request to Elasticsearch");
                }
            });

            // Also log to console for debugging
            if (context.Response.StatusCode >= 400)
            {
                _logger.LogWarning(
                    "Request {Method} {Path} responded {StatusCode} in {Duration}ms - User: {User}",
                    log.Method, log.Path, log.StatusCode, log.DurationMs, log.UserEmail ?? "Anonymous");
            }
            else
            {
                _logger.LogInformation(
                    "Request {Method} {Path} responded {StatusCode} in {Duration}ms",
                    log.Method, log.Path, log.StatusCode, log.DurationMs);
            }
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        if (!ShouldLogBody(request))
            return string.Empty;

        request.EnableBuffering();
        
        var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        
        // Reset the stream position
        request.Body.Seek(0, SeekOrigin.Begin);
        
        return bodyAsText;
    }

    private static bool ShouldLogBody(HttpRequest request)
    {
        // Don't log binary content
        var contentType = request.ContentType?.ToLower() ?? string.Empty;
        
        if (contentType.Contains("multipart/form-data") ||
            contentType.Contains("application/octet-stream") ||
            contentType.Contains("image/") ||
            contentType.Contains("video/") ||
            contentType.Contains("audio/"))
        {
            return false;
        }

        // Don't log bodies larger than 10KB
        if (request.ContentLength > 10240)
        {
            return false;
        }

        return true;
    }
}

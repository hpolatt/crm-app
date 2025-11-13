using Microsoft.Extensions.Diagnostics.HealthChecks;
using CrmApp.Core.Interfaces;

namespace CrmApp.API.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<RedisHealthCheck> _logger;

    public RedisHealthCheck(ICacheService cacheService, ILogger<RedisHealthCheck> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to set and get a test value
            var testKey = "__healthcheck__";
            var testValue = DateTime.UtcNow.ToString("o");
            
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(10));
            var retrievedValue = await _cacheService.GetAsync<string>(testKey);
            
            await _cacheService.RemoveAsync(testKey);
            
            if (retrievedValue == testValue)
            {
                var data = new Dictionary<string, object>
                {
                    { "cache", "Redis" },
                    { "status", "connected" },
                    { "roundTripSuccess", true }
                };

                _logger.LogInformation("Redis health check passed");
                
                return HealthCheckResult.Healthy("Redis cache is healthy", data);
            }
            else
            {
                _logger.LogWarning("Redis health check failed: value mismatch");
                
                return HealthCheckResult.Degraded(
                    "Redis cache returned unexpected value",
                    null,
                    new Dictionary<string, object>
                    {
                        { "cache", "Redis" },
                        { "status", "degraded" },
                        { "roundTripSuccess", false }
                    });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Redis cache is unhealthy",
                ex,
                new Dictionary<string, object>
                {
                    { "cache", "Redis" },
                    { "status", "disconnected" },
                    { "error", ex.Message }
                });
        }
    }
}

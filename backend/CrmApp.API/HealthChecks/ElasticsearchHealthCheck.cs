using Microsoft.Extensions.Diagnostics.HealthChecks;
using CrmApp.Core.Interfaces;

namespace CrmApp.API.HealthChecks;

public class ElasticsearchHealthCheck : IHealthCheck
{
    private readonly IElasticsearchService _elasticsearchService;
    private readonly ILogger<ElasticsearchHealthCheck> _logger;

    public ElasticsearchHealthCheck(
        IElasticsearchService elasticsearchService, 
        ILogger<ElasticsearchHealthCheck> logger)
    {
        _elasticsearchService = elasticsearchService;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Use PingAsync to verify Elasticsearch is reachable
            var isAlive = await _elasticsearchService.PingAsync(cancellationToken);
            
            if (isAlive)
            {
                var data = new Dictionary<string, object>
                {
                    { "searchEngine", "Elasticsearch" },
                    { "status", "connected" },
                    { "pingSuccess", true }
                };

                _logger.LogInformation("Elasticsearch health check passed");
                
                return HealthCheckResult.Healthy("Elasticsearch is healthy", data);
            }
            else
            {
                _logger.LogWarning("Elasticsearch health check failed: ping returned false");
                
                return HealthCheckResult.Degraded(
                    "Elasticsearch ping failed",
                    null,
                    new Dictionary<string, object>
                    {
                        { "searchEngine", "Elasticsearch" },
                        { "status", "degraded" },
                        { "pingSuccess", false }
                    });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Elasticsearch health check failed");
            
            // Elasticsearch being down might be degraded rather than unhealthy
            // since it's not critical for core operations
            return HealthCheckResult.Degraded(
                "Elasticsearch is unavailable",
                ex,
                new Dictionary<string, object>
                {
                    { "searchEngine", "Elasticsearch" },
                    { "status", "disconnected" },
                    { "error", ex.Message }
                });
        }
    }
}

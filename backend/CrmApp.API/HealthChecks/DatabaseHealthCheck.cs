using Microsoft.Extensions.Diagnostics.HealthChecks;
using CrmApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CrmApp.API.HealthChecks;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseHealthCheck> _logger;

    public DatabaseHealthCheck(ApplicationDbContext context, ILogger<DatabaseHealthCheck> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if database is accessible
            await _context.Database.CanConnectAsync(cancellationToken);
            
            // Execute a simple query to verify database is working
            var userCount = await _context.Users.CountAsync(cancellationToken);
            
            var data = new Dictionary<string, object>
            {
                { "database", "PostgreSQL" },
                { "status", "connected" },
                { "userCount", userCount }
            };

            _logger.LogInformation("Database health check passed. Users: {UserCount}", userCount);
            
            return HealthCheckResult.Healthy("Database is healthy", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Database is unhealthy",
                ex,
                new Dictionary<string, object>
                {
                    { "database", "PostgreSQL" },
                    { "status", "disconnected" },
                    { "error", ex.Message }
                });
        }
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PKT.Application.Configuration;
using PKT.Application.Interfaces;
using CRM.Infrastructure.Services;

namespace PKT.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Redis Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisConnection");
            options.InstanceName = "CrmApp_";
        });

        // Elasticsearch
        services.Configure<ElasticsearchSettings>(options =>
        {
            var config = configuration.GetSection("ElasticsearchSettings");
            options.Uri = config["Uri"] ?? "http://localhost:9200";
            options.DefaultIndex = config["DefaultIndex"] ?? "crm-logs";
            options.Username = config["Username"] ?? string.Empty;
            options.Password = config["Password"] ?? string.Empty;
        });
        services.AddSingleton<IElasticsearchService, ElasticsearchService>();

        // Application Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}

using CRM.Application.Configuration;
using CRM.Application.DTOs.Logging;
using CRM.Application.Interfaces;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CRM.Infrastructure.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchSettings _settings;
    private readonly ILogger<ElasticsearchService> _logger;

    public ElasticsearchService(
        IOptions<ElasticsearchSettings> settings,
        ILogger<ElasticsearchService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var clientSettings = new ElasticsearchClientSettings(new Uri(_settings.Uri))
            .DefaultIndex(_settings.DefaultIndex);

        if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
        {
            clientSettings.Authentication(new Elastic.Transport.BasicAuthentication(_settings.Username, _settings.Password));
        }

        _client = new ElasticsearchClient(clientSettings);
    }

    public async Task IndexRequestLogAsync(RequestLogDto log, CancellationToken cancellationToken = default)
    {
        try
        {
            var indexName = $"{_settings.DefaultIndex}-{DateTime.UtcNow:yyyy.MM.dd}";
            
            var response = await _client.IndexAsync(log, indexName, cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to index log to Elasticsearch: {Error}", response.DebugInformation);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error indexing log to Elasticsearch");
        }
    }

    public async Task<bool> PingAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.PingAsync(cancellationToken);
            return response.IsValidResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error pinging Elasticsearch");
            return false;
        }
    }
}

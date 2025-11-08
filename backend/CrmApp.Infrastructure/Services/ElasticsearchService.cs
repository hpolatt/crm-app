using CrmApp.Core.Configuration;
using CrmApp.Core.DTOs.Logging;
using CrmApp.Core.Interfaces;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CrmApp.Infrastructure.Services;

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

    public async Task<(List<RequestLogDto> Logs, long TotalCount)> SearchActivityLogsAsync(
        string? userId = null,
        string? action = null,
        string? path = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? minStatusCode = null,
        int? maxStatusCode = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var mustQueries = new List<Query>();

            // User ID filter
            if (!string.IsNullOrEmpty(userId))
            {
                mustQueries.Add(new TermQuery("userId") { Value = userId });
            }

            // Action filter (HTTP method)
            if (!string.IsNullOrEmpty(action))
            {
                mustQueries.Add(new TermQuery("method.keyword") { Value = action.ToUpper() });
            }

            // Path filter
            if (!string.IsNullOrEmpty(path))
            {
                mustQueries.Add(new WildcardQuery("path.keyword") { Value = $"*{path}*" });
            }

            // Date range filter
            if (startDate.HasValue || endDate.HasValue)
            {
                var dateRange = new DateRangeQuery("timestamp");
                if (startDate.HasValue)
                    dateRange.Gte = startDate.Value;
                if (endDate.HasValue)
                    dateRange.Lte = endDate.Value;
                
                mustQueries.Add(dateRange);
            }

            // Status code range filter
            if (minStatusCode.HasValue || maxStatusCode.HasValue)
            {
                var statusRange = new NumberRangeQuery("statusCode");
                if (minStatusCode.HasValue)
                    statusRange.Gte = minStatusCode.Value;
                if (maxStatusCode.HasValue)
                    statusRange.Lte = maxStatusCode.Value;
                
                mustQueries.Add(statusRange);
            }

            // Build the search request
            var searchRequest = new SearchRequest($"{_settings.DefaultIndex}-*")
            {
                Query = mustQueries.Any() 
                    ? new BoolQuery { Must = mustQueries } 
                    : new MatchAllQuery(),
                Sort = new List<SortOptions>
                {
                    SortOptions.Field(new Field("timestamp"), new FieldSort { Order = SortOrder.Desc })
                },
                From = (pageNumber - 1) * pageSize,
                Size = pageSize
            };

            var response = await _client.SearchAsync<RequestLogDto>(searchRequest, cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to search logs in Elasticsearch: {Error}", response.DebugInformation);
                return (new List<RequestLogDto>(), 0);
            }

            var logs = response.Documents.ToList();
            var totalCount = response.Total;

            return (logs, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching logs in Elasticsearch");
            return (new List<RequestLogDto>(), 0);
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

    public async Task<RequestLogDto?> GetActivityLogByRequestIdAsync(string requestId, CancellationToken cancellationToken = default)
    {
        try
        {
            var searchRequest = new SearchRequest($"{_settings.DefaultIndex}-*")
            {
                Query = new TermQuery("requestId.keyword") { Value = requestId },
                Size = 1
            };

            var response = await _client.SearchAsync<RequestLogDto>(searchRequest, cancellationToken);

            if (!response.IsValidResponse)
            {
                _logger.LogError("Failed to search log by RequestId in Elasticsearch: {Error}", response.DebugInformation);
                return null;
            }

            return response.Documents.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving activity log by RequestId");
            return null;
        }
    }
}

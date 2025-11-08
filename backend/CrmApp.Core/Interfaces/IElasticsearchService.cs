using CrmApp.Core.DTOs.Logging;

namespace CrmApp.Core.Interfaces;

public interface IElasticsearchService
{
    Task IndexRequestLogAsync(RequestLogDto log, CancellationToken cancellationToken = default);
    Task<bool> PingAsync(CancellationToken cancellationToken = default);
    Task<(List<RequestLogDto> Logs, long TotalCount)> SearchActivityLogsAsync(
        string? userId = null,
        string? action = null,
        string? path = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int? minStatusCode = null,
        int? maxStatusCode = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
    Task<RequestLogDto?> GetActivityLogByRequestIdAsync(string requestId, CancellationToken cancellationToken = default);
}

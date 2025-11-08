using CRM.Application.DTOs.Logging;

namespace CRM.Application.Interfaces;

public interface IElasticsearchService
{
    Task IndexRequestLogAsync(RequestLogDto log, CancellationToken cancellationToken = default);
    Task<bool> PingAsync(CancellationToken cancellationToken = default);
}

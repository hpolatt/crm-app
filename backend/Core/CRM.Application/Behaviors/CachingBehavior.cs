using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using CRM.Application.Interfaces;

namespace CRM.Application.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
    where TResponse : class
{
    private readonly IDistributedCache _cache;
    private readonly ICacheService _cacheService;

    public CachingBehavior(
        IDistributedCache cache,
        ICacheService cacheService)
    {
        _cache = cache;
        _cacheService = cacheService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!request.BypassCache)
        {
            var cachedResponse = await _cacheService.GetAsync<TResponse>(request.CacheKey);
            if (cachedResponse != null)
            {
                return cachedResponse;
            }
        }

        var response = await next();

        if (response != null)
        {
            await _cacheService.SetAsync(
                request.CacheKey,
                response,
                request.SlidingExpiration);
        }

        return response;
    }
}

// Marker interface for cacheable queries
public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? SlidingExpiration { get; }
    bool BypassCache { get; }
}

using EcommerceApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EcommerceApplication.PipelineBehaviors
{
    public class RedisCachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCachingBehavior<TRequest, TResponse>> _logger;

        public RedisCachingBehavior(IDistributedCache cache,
            ILogger<RedisCachingBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var cachedData = await _cache.GetStringAsync(request.CacheKey, ct);
            if (cachedData != null)
            {
                _logger.LogInformation("Checking cachedData {cachedData}", cachedData);
                return JsonSerializer.Deserialize<TResponse>(cachedData)!;
            }
            _logger.LogInformation("Cache miss for key: {CacheKey}", request.CacheKey);
            var response = await next();

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.Expiration,
            };

            await _cache.SetStringAsync(request.CacheKey, JsonSerializer.Serialize(response), options, ct);

            return response;
        }
    }
}

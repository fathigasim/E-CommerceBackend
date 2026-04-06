using EcommerceApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
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

        public RedisCachingBehavior(IDistributedCache cache) => _cache = cache;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            var cachedData = await _cache.GetStringAsync(request.CacheKey, ct);
            if (cachedData != null)
            {
                return JsonSerializer.Deserialize<TResponse>(cachedData)!;
            }

            var response = await next();

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = request.Expiration
            };

            await _cache.SetStringAsync(request.CacheKey, JsonSerializer.Serialize(response), options, ct);

            return response;
        }
    }
}

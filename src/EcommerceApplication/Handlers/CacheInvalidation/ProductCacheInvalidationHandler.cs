using EcommerceApplication.Features.Products.Notifications;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Handlers.InvalidationCache
{
    public class ProductCacheInvalidationHandler : INotificationHandler<ProductUpdatedNotification>
    {
        private readonly IMemoryCache _cache;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<ProductCacheInvalidationHandler> _logger;

        public ProductCacheInvalidationHandler(IMemoryCache cache, IConnectionMultiplexer redis, IDistributedCache distributedCache, ILogger<ProductCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
            _redis = redis;
            _distributedCache = distributedCache;
        }

        public async Task Handle(ProductUpdatedNotification notification, CancellationToken ct)
        {
            //var cacheKey = $"GetProduct-{notification.ProductId}";

            //_cache.Remove(cacheKey);

            //_logger.LogInformation("Busted cache for key: {CacheKey} due to update.", cacheKey);

            //return Task.CompletedTask;
            // 1. Remove specific product
            await _distributedCache.RemoveAsync($"product-{notification.ProductId}", ct);

            // 2. BUST ALL PAGINATED LISTS
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: "Ecommerce_ProductList-*");
            _logger.LogInformation("Busted cache for key: {CacheKey} due to update.", keys);
            foreach (var key in keys)
            {
                await _distributedCache.RemoveAsync(key, ct);
            }
        }
    }
}

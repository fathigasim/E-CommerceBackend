using EcommerceApplication.Features.Products.Notifications;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.Features.Products.InvalidationCache
{
    public class CacheInvalidationHandler : INotificationHandler<ProductUpdatedNotification>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheInvalidationHandler> _logger;

        public CacheInvalidationHandler(IMemoryCache cache, ILogger<CacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(ProductUpdatedNotification notification, CancellationToken ct)
        {
            var cacheKey = $"product-{notification.ProductId}";

            _cache.Remove(cacheKey);

            _logger.LogInformation("Busted cache for key: {CacheKey} due to update.", cacheKey);

            return Task.CompletedTask;
        }
    }
}


using MediaRTutorialApplication.Features.Products.Commands;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
namespace EcommerceApplication.PipelineBehaviors
{
 

    public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICacheInvalidatorCommand
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CacheInvalidationBehavior<TRequest, TResponse>> _logger;

        public CacheInvalidationBehavior(IMemoryCache cache, ILogger<CacheInvalidationBehavior<TRequest, TResponse>> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
        {
            // 1. Let the Command Handler finish first (save to DB)
            var response = await next();

            // 2. If we got here without an exception, clear the cache
            foreach (var key in request.CacheKeys)
            {
                _cache.Remove(key);
                _logger.LogInformation("Invalidated cache for key: {CacheKey}", key);
            }

            return response;
        }
    }
}

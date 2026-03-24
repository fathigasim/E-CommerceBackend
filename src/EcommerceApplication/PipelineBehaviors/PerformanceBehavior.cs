using MediaRTutorialApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.PipelineBehaviors
{
    // Application/Common/Behaviors/PerformanceBehavior.cs
  

    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly ITenantService _tenantService;

        public PerformanceBehavior(
            ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
            ITenantService tenantService)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _tenantService = tenantService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            var elapsedMilliseconds = _timer.ElapsedMilliseconds;

            //if (elapsedMilliseconds > 500)
            //{
            //    var requestName = typeof(TRequest).Name;
            //    var tenantId = _tenantService.HasCurrentTenant()
            //        ? _tenantService.GetCurrentTenantId()
            //        : "N/A";

            //    _logger.LogWarning(
            //        "Long Running Request: {RequestName} ({ElapsedMs}ms) - Tenant: {TenantId}",
            //        requestName, elapsedMilliseconds, tenantId);
            //}

            return response;
        }
    }
}

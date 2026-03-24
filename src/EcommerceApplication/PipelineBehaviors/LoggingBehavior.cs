using MediaRTutorialApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApplication.PipelineBehaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
      //  private readonly ITenantService _tenantService;

        public LoggingBehavior(
            ILogger<LoggingBehavior<TRequest, TResponse>> logger
            //,
            //ITenantService tenantService
            )
        {
            _logger = logger;
          //  _tenantService = tenantService;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            //var tenantId = _tenantService.HasCurrentTenant()
            //    ? _tenantService.GetCurrentTenantId()
            //    : "N/A";

            _logger.LogInformation(
                "Handling {RequestName}",requestName
               // , tenantId
                );

            //try
            //{
            var response = await next();

            //    _logger.LogInformation(
            //        "Handled {RequestName} successfully for Tenant: {TenantId}",
            //        requestName, tenantId);

               return response;
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex,
            //        "Error handling {RequestName} for Tenant: {TenantId}",
            //        requestName, tenantId);
            //    throw;
            //}
           }
        }
}

using MediaRTutorialApplication.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics; // Added for Stopwatch
namespace EcommerceApplication.PipelineBehaviors
{
   

    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            // 1. Log the start
            _logger.LogInformation("Starting Request: {RequestName}", requestName);

            var timer = Stopwatch.StartNew();

            try
            {
                var response = await next();

                // 2. Log completion with timing
                timer.Stop();
                _logger.LogInformation("Completed Request: {RequestName} in {ElapsedMilliseconds}ms",
                    requestName, timer.ElapsedMilliseconds);

                return response;
            }
            catch (Exception ex)
            {
                // 3. Optional: Log errors here, or let your Middleware handle it.
                // Logging it here gives you the RequestName context specifically for MediatR.
                timer.Stop();
                _logger.LogError(ex, "❌ Request Failed: {RequestName} after {ElapsedMilliseconds}ms",
                    requestName, timer.ElapsedMilliseconds);
                throw;
            }
        }
    }
}

using EcommerceApplication.Common.Localization;
using EcommerceApplication.Common.Localization.Resources;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace EcommerceApplication.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IStringLocalizer<SharedResource> localizer)
        {
            _next = next;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            context.Response.ContentType = "application/json";

            object response;
            int statusCode;

            //  Use fully qualified name to be 100% sure
            if (exception is FluentValidation.ValidationException validationException)
            {
                Console.WriteLine(" Matched FluentValidation.ValidationException");

                var modelState = new ModelStateDictionary();
                foreach (var error in validationException.Errors)
                {
                    Console.WriteLine($"   Adding: {error.PropertyName} = {error.ErrorMessage}");
                    modelState.AddModelError(
                        error.PropertyName ?? string.Empty,
                        error.ErrorMessage);
                }

                response = new ValidationProblemDetails(modelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = _localizer["ValidationError"],
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                };
                statusCode = StatusCodes.Status400BadRequest;
            }
            else if (exception is NotFoundException notFoundException)
            {
                response = new ProblemDetails
                {
                    Title = notFoundException.Message,
                    Status = StatusCodes.Status404NotFound
                };
                statusCode = StatusCodes.Status404NotFound;
            }
            else if (exception is UnauthorizedAccessException)
            {
                response = new ProblemDetails
                {
                    Title = exception.Message,
                    Status = StatusCodes.Status401Unauthorized
                };
                statusCode = StatusCodes.Status401Unauthorized;
            }
            else if (exception is ApiException apiException)
            {
                response = new ProblemDetails
                {
                    Title = apiException.Message,
                    Status = StatusCodes.Status502BadGateway // or 500
                };
                statusCode = StatusCodes.Status502BadGateway;
            }
            else if (exception is ApiTimeoutException timeoutException)
            {
                response = new ProblemDetails
                {
                    Title = timeoutException.Message,
                    Status = StatusCodes.Status504GatewayTimeout
                };
                statusCode = StatusCodes.Status504GatewayTimeout;
            }
            // 1. Add using System.Net.Http; at the top of your file
            else if (exception is HttpRequestException httpException)
            {
                // If StatusCode is null, it's usually a network/DNS failure (504)
                // If it has a value, the external service (GOSI/Yakeen) returned an error (502)
                statusCode = httpException.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => StatusCodes.Status401Unauthorized,
                    HttpStatusCode.Forbidden => StatusCodes.Status403Forbidden,
                    HttpStatusCode.NotFound =>StatusCodes.Status404NotFound,
                    null => StatusCodes.Status504GatewayTimeout,
                    _ => StatusCodes.Status502BadGateway
                };

                response = new ProblemDetails
                {
                    Status = statusCode,
                    // Use localizer for a generic message, or use httpException.Message for debugging
                    Title = statusCode == StatusCodes.Status504GatewayTimeout
                            ? _localizer["ExternalServiceTimeout"]
                            : _localizer["ExternalServiceError"],
                    Detail = httpException.Message // Optional: remove in production for security
                };
            }
            else
            {
                Console.WriteLine($" Unhandled: {exception.GetType().FullName}");

                response = new ProblemDetails
                {
                    Title = _localizer["InternalServerError"],
                    Status = StatusCodes.Status500InternalServerError
                };
                statusCode = StatusCodes.Status500InternalServerError;
            }

            context.Response.StatusCode = statusCode;

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
    
}
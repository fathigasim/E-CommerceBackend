using FluentValidation;
using EcommerceApplication;
using EcommerceApplication.PipelineBehaviors;

using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EcommerceApplication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            // ✅ Register AutoMapper FIRST with explicit assembly
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(assembly);
            }, assembly);
            // 1. Register all Validators from the Application assembly
            services.AddValidatorsFromAssembly(assembly);

            // 2. Register MediatR with Pipeline Behaviors
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);

                //  ValidationBehavior - runs validation before handler
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

                // Logging
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));

                // Caching
                cfg.AddOpenBehavior(typeof(CachingBehavior<,>));

                // Cache Invalidation
                cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
            });
            // Add Localization for Application Layer
           // services.AddLocalization();
            // 3. Register AutoMapper
            //services.AddAutoMapper(typeof(MappingProfile));

            return services;
        }
    }
}
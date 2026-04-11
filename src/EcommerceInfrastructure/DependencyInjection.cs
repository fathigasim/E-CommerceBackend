using EcommerceApplication.Common.Settings;
using EcommerceApplication.Interfaces;
using EcommerceDomain.Entities;
using EcommerceDomain.Interfaces;
using EcommerceInfrastructure.Identity;
using EcommerceInfrastructure.Payments;
using EcommerceInfrastructure.Persistance;
using EcommerceInfrastructure.Repository;
using EcommerceInfrastructure.Services;
using MediaRTutorialApplication.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Polly.Extensions.Http;
using StackExchange.Redis;
using Stripe;
using System.Text;
using ApplicationUser = EcommerceInfrastructure.Identity.ApplicationUser;
using IdentityService = EcommerceInfrastructure.Identity.IdentityService;

namespace EcommerceInfrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var assembly = typeof(DependencyInjection).Assembly;
            // ✅ Register AutoMapper FIRST with explicit assembly
            services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(assembly);
            }, assembly);
            services.AddMemoryCache();
            // Redis configuration
            // 1. Define your connection string
            var redisConnectionString = configuration["Redis:ConnectionString"] ?? "localhost:6379";
            // 2.Register the ConnectionMultiplexer as a Singleton
           // This fixes the "Unable to resolve service" error
            services.AddSingleton<IConnectionMultiplexer>(sp =>
             ConnectionMultiplexer.Connect(redisConnectionString));
            // 3. Register the standard IDistributedCache 
            // (Optional: It can use the existing multiplexer to save resources)
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "Ecommerce_"; // Prefixes all keys
            });
            // Stripe configuration
            var stripeSettings = configuration
                .GetSection(StripeSettings.SectionName)
                .Get<StripeSettings>()!;

            StripeConfiguration.ApiKey = stripeSettings.SecretKey;

            services.Configure<StripeSettings>(
                configuration.GetSection(StripeSettings.SectionName));

            // DbContext (REGISTER ONCE)
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // JWT options (OPTIONS PATTERN)
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));
            services.Configure<SmtpSettings>(
              configuration.GetSection("SmtpSettings"));
            // Authentication
            var jwtSection = configuration.GetSection("JwtSettings");
            var smtpSection = configuration.GetSection("SmtpSettings");
            services.Configure<JwtSettings>(jwtSection);
          

            services.Configure<SmtpSettings>(smtpSection);
            services.AddAuthentication(options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
            )
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSection["Issuer"],
                        ValidAudience = jwtSection["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            services.AddAuthorization();
            // Infrastructure services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IPagedRepository<>), typeof(PagedRepository<>));
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();


            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<IBasketContextAccessor, BasketContextAccessor>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbSeeder, DbSeeder>();
            services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IIdentityService,IdentityService>();
            services.AddHttpClient();
            services.AddScoped<IHttpClientService, HttpClientService>();
            services.AddTransient<IEmailSender,EmailSender>();
            services.AddMemoryCache();
            //services.AddScoped<IGitHubService, GitHubService>();
            //services.AddHttpClient("GitHub", client =>
            //{
            //    client.DefaultRequestHeaders.Add("User-Agent", "EcommerceApp");
            //    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            //});
            //services.AddHttpClient<GitHubService>(client =>
            //{
            //    client.DefaultRequestHeaders.Add("User-Agent", "EcommerceApp");
            //    client.BaseAddress = new Uri("https://api.github.com/");
            //    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            //});
            services.AddHttpClient<IGitHubService, GitHubService>(client =>
            {
                client.BaseAddress = new Uri("https://api.github.com/");
                client.DefaultRequestHeaders.Add("User-Agent", "EcommerceApp");
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            }).AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetCircuitBreakerPolicy());
            //.AddTransientHttpErrorPolicy(policy =>
            //   policy.WaitAndRetryAsync(3, retryAttempt =>
            //   TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));
           


            static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
            {
                return HttpPolicyExtensions
                    .HandleTransientHttpError()
                   //.OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                    .WaitAndRetryAsync(
                        3,
                        retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (outcome, timespan, retryCount, context) =>
                        {
                            Console.WriteLine($"Retry {retryCount} after {timespan.TotalSeconds}s");
                        }
                    );
            }

            static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
            {
                return HttpPolicyExtensions
                    .HandleTransientHttpError().CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
                    //.CircuitBreakerAsync(
                    //    handledEventsAllowedBeforeBreaking: 5,
                    //    durationOfBreak: TimeSpan.FromSeconds(30),
                    //    onBreak: (outcome, timespan) =>
                    //    {
                    //        Console.WriteLine($"Circuit breaker opened for {timespan.TotalSeconds}s");
                    //    },
                    //    onReset: () =>
                    //    {
                    //        Console.WriteLine("Circuit breaker reset");
                    //    }
                    //);
            };


            //services.AddHttpClient<IGitHubService, GitHubService>(client =>
            //{
            //    client.BaseAddress = new Uri("https://api.github.com/");
            //    client.DefaultRequestHeaders.Add("User-Agent", "EcommerceApp");
            //    client.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

            //    var username = configuration["GitHub:Username"];
            //    var password = configuration["GitHub:Password"];
            //    var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

            //    client.DefaultRequestHeaders.Authorization =
            //        new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
            //});
            return services;
        }
    }
}
using EcommerceApplication;
using EcommerceApplication;
using EcommerceApplication.Exceptions;
using EcommerceInfrastructure;
using EcommerceInfrastructure.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//  1. Add Application layer (includes MediatR, Validators, Behaviors)
builder.Services.AddApplication();

//  2. Add Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

//using Application.Common.Validators;
//using FluentValidation;

//// Register FluentValidation with Arabic Language Manager
//ValidatorOptions.Global.LanguageManager = new ArabicLanguageManager();

//// Or set default culture
//ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("ar");
// Configure Localization
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = ""; // Resources are in the same folder as classes
});

// Configure supported cultures
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ar")
    };

    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    // Add culture providers (order matters)
    options.RequestCultureProviders = new List<IRequestCultureProvider>
    {
        new QueryStringRequestCultureProvider(),      // ?culture=ar
        new CookieRequestCultureProvider(),           // Cookie
        new AcceptLanguageHeaderRequestCultureProvider() // Accept-Language header
    };
});
//  3. Add Controllers ONCE with all configurations
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        // Disable automatic model state validation
        options.SuppressModelStateInvalidFilter = true;
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });


builder.Services.AddMemoryCache();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "https://localhost:5173",
                    "http://localhost:5174",
                    "https://localhost:5174",
                    "http://localhost:3000",
                    "https://localhost:3000"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        }
        else
        {
            policy.WithOrigins("https://yourdomain.com")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token in the format: Bearer {your_token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrUser", policy =>
        policy.RequireRole("Admin", "User"));
});
var app = builder.Build();


// Use Request Localization Middleware (MUST be before other middleware)
app.UseRequestLocalization();
// Seed database
await app.SeedDatabaseAsync();

//  Exception middleware FIRST (before other middleware)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Static Files
app.UseDefaultFiles();
app.UseStaticFiles();

var imgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
Console.WriteLine("Serving static images from: " + imgPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products")),
    RequestPath = "/StaticImages"
});

app.UseRouting();
app.UseCors("DevCors");
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
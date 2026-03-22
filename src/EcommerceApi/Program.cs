using EcommerceApplication.Exceptions;
using EcommerceInfrastructure;
using EcommerceInfrastructure.Extensions;
using MediaRTutorialApplication;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//  1. Add Application layer (includes MediatR, Validators, Behaviors)
builder.Services.AddApplication();

//  2. Add Infrastructure layer
builder.Services.AddInfrastructure(builder.Configuration);

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

//  REMOVE THESE - Already in AddApplication():
// builder.Services.AddFluentValidationAutoValidation(...);
// builder.Services.AddValidatorsFromAssemblyContaining<...>();
// builder.Services.AddMediatR(...);

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

var app = builder.Build();

// Seed database
await app.SeedDatabaseAsync();

//  Exception middleware FIRST (before other middleware)
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DevCors");

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

app.UseAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOrUser", policy =>
        policy.RequireRole("Admin", "User"));
});
app.MapControllers();

app.Run();
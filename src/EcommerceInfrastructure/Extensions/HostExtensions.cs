using EcommerceInfrastructure.Persistance;
using MediaRTutorialApplication.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceInfrastructure.Extensions
{
    public static class HostExtensions
    {
        public static async Task<IHost> SeedDatabaseAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContext>();

                // THIS LINE IS KEY: It applies any pending migrations 
                // and creates the database/tables if they don't exist.
                await context.Database.MigrateAsync();
                var seeder = services.GetRequiredService<IDbSeeder>();
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<IDbSeeder>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }

            return host;
        }
    }
}

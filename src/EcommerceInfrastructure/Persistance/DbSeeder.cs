using EcommerceDomain.Entities;
using EcommerceInfrastructure.Identity;
using MediaRTutorialApplication.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace EcommerceInfrastructure.Persistance
{
    public class DbSeeder : IDbSeeder
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DbSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                // Apply pending migrations
                await _context.Database.MigrateAsync();

                // Seed roles
                await SeedRolesAsync();

                // Seed users
                await SeedUsersAsync();

                // Seed other data
              //  await SeedOtherDataAsync();

                _logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        public async Task SeedRolesAsync()
        {
            string[] roleNames = { "Admin", "User", "Manager", "Moderator" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    _logger.LogInformation($"Role '{roleName}' created");
                }
            }
        }

        public async Task SeedUsersAsync()
        {
            // Seed admin user
            if (!await _userManager.Users.AnyAsync(u => u.Email == "admin@example.com"))
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@example.com",
                    EmailConfirmed = true,
                    FirstName = "System",
                    LastName = "Administrator",
                    DateCreated = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(adminUser, "Admin@123456");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    _logger.LogInformation("Admin user created successfully");
                }
            }

            // Seed test users
            var testUsers = new[]
            {
                new { Email = "manager@example.com", Password = "Manager@123456", Role = "Manager", FirstName = "John", LastName = "Manager" },
                new { Email = "user@example.com", Password = "User@123456", Role = "User", FirstName = "Jane", LastName = "User" }
            };

            foreach (var testUser in testUsers)
            {
                if (!await _userManager.Users.AnyAsync(u => u.Email == testUser.Email))
                {
                    var user = new ApplicationUser
                    {
                        UserName = testUser.Email.Split('@')[0],
                        Email = testUser.Email,
                        EmailConfirmed = true,
                        FirstName = testUser.FirstName,
                        LastName = testUser.LastName,
                        DateCreated = DateTime.UtcNow,
                        IsActive = true
                    };

                    var result = await _userManager.CreateAsync(user, testUser.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, testUser.Role);
                        _logger.LogInformation($"User '{testUser.Email}' created successfully");
                    }
                }
            }
        }

        //private async Task SeedOtherDataAsync()
        //{
        //    // Add any other seed data here
        //    // For example: Categories, Products, etc.

        //    if (!await _context.Set<Category>().AnyAsync())
        //    {
        //        var categories = new[]
        //        {
        //            new Category { Name = "Electronics", Description = "Electronic items" },
        //            new Category { Name = "Books", Description = "Books and publications" },
        //            new Category { Name = "Clothing", Description = "Clothes and accessories" }
        //        };

        //        await _context.Set<Category>().AddRangeAsync(categories);
        //        await _context.SaveChangesAsync();
        //        _logger.LogInformation("Categories seeded successfully");
        //    }
        //}
    }
}

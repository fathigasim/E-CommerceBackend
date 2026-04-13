using EcommerceDomain.Common;
using EcommerceDomain.Entities;
using EcommerceInfrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace EcommerceInfrastructure.Persistance
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;
        public AppDbContext(DbContextOptions<AppDbContext> options
            , IHttpContextAccessor? httpContextAccessor = null)  // Optional for testing
             : base(options) {
        _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //  REQUIRED

     
            

            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<OrderItem>(entity =>
            {
                // Primary Key
                entity.HasKey(oi => oi.Id);
                // One-to-Many with Order
                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.Items)
                    .HasForeignKey(oi => oi.OrderId)
                    .IsRequired()                           // OrderItem must have Order
                    .OnDelete(DeleteBehavior.Cascade);      // Delete OrderItems when Order deleted
                // One-to-One with Product (optional)
                entity.HasOne(oi => oi.Product)
                    .WithMany(it=>it.OrderItems)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.NoAction);     // Set ProductId to null if Product deleted
            });
            modelBuilder.Entity<Payment>(entity =>
            {
                // Primary Key
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Amount)
          .HasColumnType("decimal(18,2)")
          .IsRequired();
                // One-to-One with Order
                entity.HasOne(p => p.Order)
                    .WithMany(o => o.Payments)
                    .HasForeignKey(p => p.OrderId)
                    .IsRequired(false)                           // Payment must have Order not at first but can be null
                    .OnDelete(DeleteBehavior.Cascade);
                // Delete Payment when Order deleted
                //entity.HasIndex(p => p.Id)
                //      .IsUnique()
                //      .HasFilter("[PaymentIntentId] IS NOT NULL");

                //    entity.HasIndex(p => p.UserId);
                // Unique index (enforces one-to-one at DB level)
                entity.HasIndex(p => p.OrderId)
                    .IsUnique();
            });
            //modelBuilder.Entity<Order>().HasIndex(o => o.ProductName).IsUnique();
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.RefreshToken).HasMaxLength(500);
            });

            // Seed data
            SeedData(modelBuilder);
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Category> Categories { get; set; }


        public DbSet<Basket> Basket { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<Payment> Payments { get; set; }






        private static void SeedData(ModelBuilder builder)
        {
            var electronicsId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var clothingId = Guid.Parse("22222222-2222-2222-2222-222222222222");

            builder.Entity<Category>().HasData(
                new Category { Id = electronicsId, Name = "Electronics", Description = "Electronic devices" },
                new Category { Id = clothingId, Name = "Clothing", Description = "Apparel and accessories" }
            );

            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Laptop",
                    Description = "High-performance laptop",
                    Price = 999.99m,
                    StockQuantity = 50,
                    CategoryId = electronicsId
                },
                new Product
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "T-Shirt",
                    Description = "Cotton t-shirt",
                    Price = 29.99m,
                    StockQuantity = 200,
                    CategoryId = clothingId
                }
            );
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor?.HttpContext?.User?
              .FindFirstValue(ClaimTypes.NameIdentifier);
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        entry.Entity.CreatedBy=userId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        entry.Entity.UpdatedBy=userId;
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

    } 
}

//modelBuilder.Entity<Payment>(entity =>
//{
//    entity.HasKey(p => p.Id);

//    entity.Property(p => p.Amount)
//          .HasColumnType("decimal(18,2)")
//          .IsRequired();

//    entity.Property(p => p.PaymentIntentId)
//          .HasMaxLength(255);

//    // One-to-One: Order -> Payment (optional)
//    entity.HasOne(p => p.Order)
//          .WithOne()
//          .HasForeignKey<Payment>(p => p.OrderId)
//          .OnDelete(DeleteBehavior.SetNull);

//    entity.HasIndex(p => p.PaymentIntentId)
//          .IsUnique()
//          .HasFilter("[PaymentIntentId] IS NOT NULL");

//    entity.HasIndex(p => p.UserId);
//});

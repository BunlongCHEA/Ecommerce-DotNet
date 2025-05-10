using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ECommerceAPI.Models;

namespace ECommerceAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<LocationCountry> LocationCountries { get; set; }
        public DbSet<LocationRegion> LocationRegions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<CouponUserList> CouponUserLists { get; set; }
        public DbSet<Order> Orders { get; set; } 
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ShipmentType> ShipmentTypes { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Product entity
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.SubCategory)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SubCategoryId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Store)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.StoreId);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Coupon)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CouponId);

            // Configure the CouponUserList entity
            modelBuilder.Entity<CouponUserList>()
                .HasOne(c => c.Coupon)
                .WithMany(c => c.CouponUserLists)
                .HasForeignKey(c => c.CouponId);
            
            modelBuilder.Entity<CouponUserList>()
                .HasOne(c => c.ApplicationUser)
                .WithMany(u => u.CouponUserLists)
                .HasForeignKey(c => c.UserId);

            // Configure the Location entity
            // modelBuilder.Entity<Location>()
            //     .HasOne(l => l.LocationCountry)
            //     .WithMany(c => c.Locations)
            //     .HasForeignKey(l => l.CountryId);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.ApplicationUser)
                .WithMany(u => u.Locations)
                .HasForeignKey(l => l.UserId);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.LocationRegion)
                .WithMany(r => r.Locations)
                .HasForeignKey(l => l.RegionId);

            // Configure the LocationCountry entity
            modelBuilder.Entity<LocationRegion>()
                .HasOne(l => l.LocationCountry)
                .WithMany(c => c.LocationRegions)
                .HasForeignKey(l => l.CountryId);

            // Configure the Order entity
            // modelBuilder.Entity<Order>()
            //     .HasOne(o => o.Shipment)
            //     .WithMany(s => s.Orders)
            //     .HasForeignKey(o => o.ShipmentId);
            
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Shipment)
                .WithOne(s => s.Order)
                .HasForeignKey<Order>(o => o.ShipmentId)
                .OnDelete(DeleteBehavior.NoAction); // Set up the one-to-one relationship with null when delete

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.PaymentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.CouponUserList)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CouponUserListId);
            
            // modelBuilder.Entity<Order>()
            //     .HasOne(o => o.ApplicationUser)
            //     .WithMany(u => u.Orders)
            //     .HasForeignKey(o => o.UserId);
            
            // Configure the OrderItem entity
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);

            // Configure the Shipment entity
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.ShipmentType)
                .WithMany(st => st.Shipments)
                .HasForeignKey(s => s.ShipmentTypeId);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Location)
                .WithMany(l => l.Shipments)
                .HasForeignKey(s => s.LocationId);

            // modelBuilder.Entity<Shipment>()
            //     .HasOne(s => s.ApplicationUser)
            //     .WithMany(u => u.Shipments)
            //     .HasForeignKey(s => s.UserId);

            // Configure the Payment entity
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.ApplicationUser)
                .WithMany(u => u.Payments)
                .HasForeignKey(p => p.UserId);
        }
    }
}
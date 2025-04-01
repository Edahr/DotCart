using DotCart.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DotCart.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoreBrand> StoreBrands { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //USER CONFIGURATION
            // One User -> Many Stores (Cascade delete: Deleting a user deletes their stores)
            modelBuilder.Entity<User>()
                .HasMany(u => u.Stores)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            //STORE CONFIGURATION
            // One Store -> Many Addresses (Cascade delete: Deleting a store deletes addresses)
            modelBuilder.Entity<Store>()
                .HasMany(s => s.Addresses)
                .WithOne(a => a.Store)
                .HasForeignKey(a => a.StoreID)
                .OnDelete(DeleteBehavior.Cascade);

            // One Store -> Many Products (Cascade delete: Deleting a store deletes products)
            modelBuilder.Entity<Store>()
                .HasMany(s => s.Products)
                .WithOne(p => p.Store)
                .HasForeignKey(p => p.StoreID)
                .OnDelete(DeleteBehavior.Cascade);

            // Many-to-Many: Store <-> Brand via StoreBrand
            modelBuilder.Entity<StoreBrand>()
                .HasKey(sb => new { sb.StoreID, sb.BrandID }); // Composite primary key

            modelBuilder.Entity<StoreBrand>()
                .HasOne(sb => sb.Store)
                .WithMany(s => s.StoreBrands)
                .HasForeignKey(sb => sb.StoreID)
                .OnDelete(DeleteBehavior.Cascade); // Optional: delete StoreBrand entries when Store is deleted

            modelBuilder.Entity<StoreBrand>()
                .HasOne(sb => sb.Brand)
                .WithMany(b => b.StoreBrands)
                .HasForeignKey(sb => sb.BrandID)
                .OnDelete(DeleteBehavior.Cascade); // Optional: delete StoreBrand entries when Brand is deleted

            //BRAND CONFIGURATION
            // One Brand -> Many Products (Cascade delete: Deleting a brand deletes products)
            modelBuilder.Entity<Brand>()
                .HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

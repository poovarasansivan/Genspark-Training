using System.Collections.Generic;
using MigratedApi.Models;
using MigratedApi.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace MigratedApi.Contexts
{
    public class ShopDbContext : DbContext
    {
        internal object _productRepository;

        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CartResponseDto> CartResponseDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartResponseDto>().HasNoKey();                
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId);

            // User - News relationship
            modelBuilder.Entity<News>()
                .HasOne(n => n.User)
                .WithMany(u => u.News)
                .HasForeignKey(n => n.UserId);

            // Product - Category relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Product)
                .HasForeignKey(p => p.CategoryId);

            // Product - Color relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Color)
                .WithMany(c => c.Product)
                .HasForeignKey(p => p.ColorId);

            // Product - Model relationship
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Model)
                .WithMany(m => m.Product)
                .HasForeignKey(p => p.ModelId);
        }
    }
}

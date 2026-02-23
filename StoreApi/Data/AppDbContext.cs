using Microsoft.EntityFrameworkCore;
using StoreApi.Models.Identity;
using StoreApi.Models.Store;

namespace StoreApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }  
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics" },
                new Category { Id = 2, Name = "Accessories" }
                );  

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Wireless", Products = null },
                new Tag { Id = 2, Name = "RGB", Products = null },
                new Tag { Id = 3, Name = "Gaming", Products = null }
                );

        }
    }
}

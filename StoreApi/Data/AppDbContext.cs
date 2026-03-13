using Microsoft.EntityFrameworkCore;
using StoreApi.Models.Identity;
using StoreApi.Models.Store;
using System.Reflection;

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
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            var electronicsId = new Guid("11111111-1111-1111-1111-111111111111");
            var accessoriesId = new Guid("22222222-2222-2222-2222-222222222222");

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = electronicsId, Name = "Electronics" },
                new Category { Id = accessoriesId, Name = "Accessories" }
                );

            var wirelessId = new Guid("33333333-3333-3333-3333-333333333333");
            var rgbId = new Guid("44444444-4444-4444-4444-444444444444");
            var gamingId = new Guid("55555555-5555-5555-5555-555555555555");

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = wirelessId, Name = "Wireless" },
                new Tag { Id = rgbId, Name = "RGB" },
                new Tag { Id = gamingId, Name = "Gaming" }
                );
        }
    }
}

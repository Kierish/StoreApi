using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                                ?? Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Server=fake;Database=fake;User Id=fake;Password=fake;TrustServerCertificate=True;";
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
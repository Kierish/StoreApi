using System.Data.Common;
using Infrastructure.Data; 
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Testcontainers.MsSql;
using Xunit;

namespace IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        private DbConnection _dbConnection = default!;
        private Respawner _respawner = default!;

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            _dbConnection = new SqlConnection(_dbContainer.GetConnectionString());
            await _dbConnection.OpenAsync();

            using var scope = Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();

            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
                DbAdapter = DbAdapter.SqlServer
            });
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<DbContextOptions<AppDbContext>>();

                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(_dbContainer.GetConnectionString());
                });
            });
        }

        public async Task ResetDatabaseAsync()
        {
            if (_respawner is null || _dbConnection is null)
            {
                throw new InvalidOperationException("Test database fixture is not initialized.");
            }

            await _respawner.ResetAsync(_dbConnection);
        }

        public new async Task DisposeAsync()
        {
            if(_dbConnection is not null) 
            {
                await _dbConnection.DisposeAsync();
            }

            if (_dbContainer is not null)
            {
                await _dbContainer.StopAsync();
                await _dbContainer.DisposeAsync();  
            }

            await base.DisposeAsync();
        }
    }
}

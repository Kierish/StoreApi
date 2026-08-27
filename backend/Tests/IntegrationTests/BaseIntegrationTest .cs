using Domain.Constants;
using Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly IServiceScope _scope;

        protected readonly HttpClient Client;
        protected readonly AppDbContext DbContext;
        protected readonly IConfiguration Configuration;

        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            Client = factory.CreateClient();

            _scope = factory.Services.CreateScope();
            DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

            Configuration = _factory.Services.GetRequiredService<IConfiguration>();
        }

        protected void Authenticate(string role, Guid? userId = null)
        {
            var token = TestTokenProvider.GenerateToken(
                Configuration.GetRequiredSection("JwtSettings"), 
                userId ?? Guid.CreateVersion7(), 
                role
                );
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync()
        {
            _scope.Dispose();
            return Task.CompletedTask;
        }
    }
}

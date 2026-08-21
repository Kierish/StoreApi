using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            Client = factory.CreateClient();

            _scope = factory.Services.CreateScope();
            DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();
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

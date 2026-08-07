using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICacheService
    {
        Task SetCacheAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<T?> GetCacheAsync<T>(string key, CancellationToken cancellationToken = default);
        Task<T?> GetOrCreateAsync<T>(
            string key, Func<Task<T?>> factory, TimeSpan? expirationTime = null, CancellationToken cancellationToken = default);
        Task IncrementVersionAsync(string key);
        Task<int> GetListCurrentVersionAsync(string key, CancellationToken cancellationToken = default);
        Task RemoveCacheAsync(string key);
    }
}

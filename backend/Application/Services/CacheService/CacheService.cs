using Application.Interfaces.Services;
using Domain.Constants;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };
            var serializedValue = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, serializedValue, options);
        }

        public async Task<T?> GetCacheAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var serializedValue = await _cache.GetStringAsync(key, cancellationToken);

            if (serializedValue is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(serializedValue);
        }

        public async Task<T?> GetOrCreateAsync<T>(
            string key, 
            Func<Task<T?>> factory, 
            TimeSpan? expirationTime = null, 
            CancellationToken cancellationToken = default)
        {
            var cachedData = await GetCacheAsync<T>(key, cancellationToken);
            if (cachedData is not null)
            {
                return cachedData;
            }

            // TODO: Add CancellationToken support to the factory delegate for SQL fallback cancellation.
            var freshData = await factory();

            if (freshData is not null)
            {
                await SetCacheAsync(key, freshData, expirationTime);
            }

            return freshData;
        }

        public async Task IncrementVersionAsync(string key)
        {
            int currentVer = await GetListCurrentVersionAsync(key);
            await _cache.SetStringAsync(key, (currentVer + 1).ToString());
        }

        public async Task<int> GetListCurrentVersionAsync(string key, CancellationToken cancellationToken = default)
        {
            var currentStr = await _cache.GetStringAsync(key, cancellationToken);
            int currentVer = string.IsNullOrEmpty(currentStr) ? 1 : int.Parse(currentStr);

            return currentVer;
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}

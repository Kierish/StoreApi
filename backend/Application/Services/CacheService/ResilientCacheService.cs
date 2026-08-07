using Application.Interfaces.Services;
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
using System.Threading;
using System.Threading.Tasks;

namespace Application.Services.CacheService
{
    public class ResilientCacheService : ICacheService
    {
        private readonly ICacheService _inner;
        private readonly ILogger<ResilientCacheService> _logger;
        private readonly ResiliencePipeline _pipeline;

        public ResilientCacheService(ICacheService inner, 
            ILogger<ResilientCacheService> logger)
        {
            _inner = inner;
            _logger = logger;
            _pipeline = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(10),
                    MinimumThroughput = 3,
                    BreakDuration = TimeSpan.FromSeconds(30),
                    ShouldHandle = new PredicateBuilder().Handle<Exception>(ex =>
                        ex is RedisException or SocketException or TimeoutException)
                }).Build();
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                await _pipeline.ExecuteAsync(async ct =>
                {
                    await _inner.SetCacheAsync(key, value, expiration);
                });
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit is OPEN. Skipping Redis write for key: {Key}", key);
            }
            catch (Exception ex) when (ex is RedisException or SocketException or TimeoutException)
            {
                _logger.LogWarning(ex, "Failed to write to Redis for key: {Key}", key);
            }
        }
        public async Task<T?> GetCacheAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async ct =>
                {
                    return await _inner.GetCacheAsync<T>(key, ct);
                }, cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit is OPEN. Skipping Redis call for key: {Key}", key);
                return default;
            }
            catch (Exception ex) when (ex is RedisException or SocketException or JsonException or TimeoutException)
            {
                _logger.LogWarning(ex, "Redis is offline or failed. Falling back to database for key: {Key}", key);

                return default;
            }
        }
        public async Task<T?> GetOrCreateAsync<T>(
            string key, 
            Func<Task<T?>> factory, 
            TimeSpan? expirationTime = null, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async ct =>
                {
                    return await _inner.GetOrCreateAsync(key, factory, expirationTime, ct);
                }, cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit is OPEN. Skipping GetOrCreate for key: {Key}. Falling back directly to database.", key);

                return await factory();
            }
            catch (Exception ex) when (ex is RedisException or SocketException or TimeoutException)
            {
                _logger.LogWarning(ex, "Redis failed during GetOrCreate for key: {Key}. Falling back to database.", key);

                return await factory();
            }
        }
        public async Task IncrementVersionAsync(string key)
        {
            try
            {
                await _pipeline.ExecuteAsync(async ct =>
                {
                    await _inner.IncrementVersionAsync(key);
                });
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit is OPEN. Skipping list version increment for key: {Key}", key);
            }
            catch (Exception ex) when (ex is RedisException or SocketException or TimeoutException)
            {
                _logger.LogWarning(ex, "Failed to increment list version key: {Key}. Users may see temporarily stale list data.", key);
            }
        }
        public async Task<int> GetListCurrentVersionAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _pipeline.ExecuteAsync(async ct =>
                {
                    return await _inner.GetListCurrentVersionAsync(key, ct);
                }, cancellationToken);
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit is OPEN. Skipping version read for key: {Key}. Defaulting to 1.", key);
                return 1;
            }
            catch (Exception ex) when (ex is RedisException or SocketException or TimeoutException)
            {
                _logger.LogWarning(ex, "Failed to increment list version key: {Key}. Users may see temporarily stale list data.", key);
                return 1;
            }
        }
        public async Task RemoveCacheAsync(string key)
        {
            try
            {
                await _pipeline.ExecuteAsync(async ct =>
                {
                    await _inner.RemoveCacheAsync(key);
                });
            }
            catch (BrokenCircuitException)
            {
                _logger.LogWarning("Circuit is OPEN. Skipping cache removal for key: {Key}", key);
            }
            catch (Exception ex) when (ex is RedisException or SocketException or TimeoutException)
            {
                _logger.LogWarning(ex, "Failed to remove key: {Key} from Redis. Cached data may be stale until TTL expiration.", key);
            }
        }
    }
}

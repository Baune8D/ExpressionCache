using System;
using System.Text.Json;
using System.Threading.Tasks;
using ExpressionCache.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace ExpressionCache.Distributed;

public class DistributedCacheProvider : IExpressionCacheProvider
{
    public DistributedCacheProvider(IDistributedCache cache)
    {
        Cache = cache;
    }

    public IDistributedCache Cache { get; }

    public CacheResult<TResult> Get<TResult>(string key)
    {
        return CreateCacheResult<TResult>(Cache.GetString(key));
    }

    public async Task<CacheResult<TResult>> GetAsync<TResult>(string key)
    {
        return CreateCacheResult<TResult>(await Cache.GetStringAsync(key).ConfigureAwait(false));
    }

    public void Set<TResult>(string key, TResult value, TimeSpan expiry)
    {
        Cache.SetString(key, JsonSerializer.Serialize(value), GetOptions(expiry));
    }

    public async Task SetAsync<TResult>(string key, TResult value, TimeSpan expiry)
    {
        await Cache.SetStringAsync(key, JsonSerializer.Serialize(value), GetOptions(expiry)).ConfigureAwait(false);
    }

    private static CacheResult<TResult> CreateCacheResult<TResult>(string entry)
    {
        if (entry == null)
        {
            return CacheResult<TResult>.Failure();
        }

        return new CacheResult<TResult>
        {
            Success = true,
            Content = JsonSerializer.Deserialize<TResult>(entry),
        };
    }

    private static DistributedCacheEntryOptions GetOptions(TimeSpan expiry)
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry,
        };
    }
}

using System;
using System.Threading.Tasks;
using ExpressionCache.Core;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ExpressionCache.Distributed
{
    public class DistributedCacheProvider : IExpressionCacheProvider
    {
        public IDistributedCache Cache;

        public DistributedCacheProvider(IDistributedCache cache)
        {
            Cache = cache;
        }

        #region ---- Provider Functions ----

        public CacheResult<TResult> Get<TResult>(string key)
        {
            return CreateCacheResult<TResult>(Cache.GetString(key));
        }

        public async Task<CacheResult<TResult>> GetAsync<TResult>(string key)
        {
            return CreateCacheResult<TResult>(await Cache.GetStringAsync(key));
        }

        public void Set<TResult>(string key, TResult value, TimeSpan expiry)
        {
            Cache.SetString(key, JsonConvert.SerializeObject(value), GetOptions(expiry));
        }

        public async Task SetAsync<TResult>(string key, TResult value, TimeSpan expiry)
        {
            await Cache.SetStringAsync(key, JsonConvert.SerializeObject(value), GetOptions(expiry));
        }

        #endregion

        private static CacheResult<TResult> CreateCacheResult<TResult>(string entry)
        {
            if (entry == null) return CacheResult<TResult>.Failure();

            return new CacheResult<TResult>
            {
                Success = true,
                Content = JsonConvert.DeserializeObject<TResult>(entry)
            };
        }

        private static DistributedCacheEntryOptions GetOptions(TimeSpan expiry)
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };
        }
    }
}


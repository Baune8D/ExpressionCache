using System;
using System.Threading.Tasks;
using ExpressionCache.Core;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ExpressionCache.Redis
{
    public class RedisCacheProvider : IExpressionCacheProvider
    {
        private readonly ConnectionMultiplexer _redis;
        public IDatabase Database => _redis.GetDatabase();

        public RedisCacheProvider(ConfigurationOptions options)
        {
            _redis = ConnectionMultiplexer.Connect(options);
        }

        #region ---- Provider Functions ----

        public CacheResult<TResult> Get<TResult>(string key)
        {
            return CreateCacheResult<TResult>(Database.StringGet(key));
        }

        public async Task<CacheResult<TResult>> GetAsync<TResult>(string key)
        {
            return CreateCacheResult<TResult>(await Database.StringGetAsync(key));
        }

        public void Set<TResult>(string key, TResult value, TimeSpan expiry)
        {
            Database.StringSet(key, JsonConvert.SerializeObject(value), expiry);
        }

        public async Task SetAsync<TResult>(string key, TResult value, TimeSpan expiry)
        {
            await Database.StringSetAsync(key, JsonConvert.SerializeObject(value), expiry);
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
    }
}


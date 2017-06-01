using System;
using System.Threading.Tasks;

namespace ExpressionCache.Core
{
    public interface IExpressionCacheProvider
    {
        CacheResult<TResult> Get<TResult>(string key);
        Task<CacheResult<TResult>> GetAsync<TResult>(string key);

        void Set<TResult>(string key, TResult value, TimeSpan expiry);
        Task SetAsync<TResult>(string key, TResult value, TimeSpan expiry);
    }
}

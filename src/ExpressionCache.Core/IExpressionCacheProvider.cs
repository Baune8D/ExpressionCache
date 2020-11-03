using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ExpressionCache.Core
{
    public interface IExpressionCacheProvider
    {
        [SuppressMessage("	Microsoft.Naming", "CA1716", Justification = "Allowed")]
        CacheResult<TResult> Get<TResult>(string key);

        Task<CacheResult<TResult>> GetAsync<TResult>(string key);

        [SuppressMessage("Microsoft.Naming", "CA1716", Justification = "Allowed")]
        void Set<TResult>(string key, TResult value, TimeSpan expiry);

        Task SetAsync<TResult>(string key, TResult value, TimeSpan expiry);
    }
}

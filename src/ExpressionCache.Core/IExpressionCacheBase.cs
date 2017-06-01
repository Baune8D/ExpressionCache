using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ExpressionCache.Core
{
    public interface IExpressionCacheBase
    {
        string GetKey<TResult>(Expression<Func<TResult>> expression);
        string GetKey<TResult>(Expression<Func<Task<TResult>>> expression);

        TResult InvokeCache<TResult>(Expression<Func<TResult>> expression, TimeSpan expiry, CacheAction cacheAction);
        Task<TResult> InvokeCacheAsync<TResult>(Expression<Func<Task<TResult>>> expression, TimeSpan expiry, CacheAction cacheAction);
    }
}

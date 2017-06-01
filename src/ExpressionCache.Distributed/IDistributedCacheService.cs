using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressionCache.Core;

namespace ExpressionCache.Distributed
{
    public interface IDistributedCacheService : IExpressionCacheBase
    {
        void Remove<TResult>(Expression<Func<TResult>> expression);
        void Remove<TResult>(Expression<Func<Task<TResult>>> expression);
        Task RemoveAsync<TResult>(Expression<Func<TResult>> expression);
        Task RemoveAsync<TResult>(Expression<Func<Task<TResult>>> expression);

        bool Exists<TResult>(Expression<Func<TResult>> expression);
        bool Exists<TResult>(Expression<Func<Task<TResult>>> expression);
        Task<bool> ExistsAsync<TResult>(Expression<Func<TResult>> expression);
        Task<bool> ExistsAsync<TResult>(Expression<Func<Task<TResult>>> expression);

        TResult Get<TResult>(Expression<Func<TResult>> expression);
        TResult Get<TResult>(Expression<Func<Task<TResult>>> expression);
        Task<TResult> GetAsync<TResult>(Expression<Func<TResult>> expression);
        Task<TResult> GetAsync<TResult>(Expression<Func<Task<TResult>>> expression);

        Task<IEnumerable<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<TResult>>> expressions);
        Task<IEnumerable<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<Task<TResult>>>> expressions);

        void Set<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry);
        void Set<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry);
        Task SetAsync<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry);
        Task SetAsync<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry);
    }
}

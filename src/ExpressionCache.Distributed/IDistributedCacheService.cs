using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressionCache.Core;

namespace ExpressionCache.Distributed;

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

    [SuppressMessage("	Microsoft.Naming", "CA1716", Justification = "Allowed")]
    TResult Get<TResult>(Expression<Func<TResult>> expression);

    [SuppressMessage("	Microsoft.Naming", "CA1716", Justification = "Allowed")]
    TResult Get<TResult>(Expression<Func<Task<TResult>>> expression);

    Task<TResult> GetAsync<TResult>(Expression<Func<TResult>> expression);

    Task<TResult> GetAsync<TResult>(Expression<Func<Task<TResult>>> expression);

    Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<TResult>>> expressions);

    Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<Task<TResult>>>> expressions);

    [SuppressMessage("	Microsoft.Naming", "CA1716", Justification = "Allowed")]
    void Set<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry);

    [SuppressMessage("	Microsoft.Naming", "CA1716", Justification = "Allowed")]
    void Set<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry);

    Task SetAsync<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry);

    Task SetAsync<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry);
}

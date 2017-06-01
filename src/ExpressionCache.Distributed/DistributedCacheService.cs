using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressionCache.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace ExpressionCache.Distributed
{
    public class DistributedCacheService : ExpressionCacheBase, IDistributedCacheService
    {
        protected IDistributedCache Cache => ((DistributedCacheProvider) Provider).Cache;

        public DistributedCacheService(IDistributedCache cache) : base(new DistributedCacheProvider(cache)) { }

        public void Remove<TResult>(Expression<Func<TResult>> expression)
        {
            Cache.Remove(GetKey(expression));
        }

        public void Remove<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            Cache.Remove(GetKey(expression));
        }

        public async Task RemoveAsync<TResult>(Expression<Func<TResult>> expression)
        {
            await Cache.RemoveAsync(GetKey(expression));
        }

        public async Task RemoveAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            await Cache.RemoveAsync(GetKey(expression));
        }

        public bool Exists<TResult>(Expression<Func<TResult>> expression)
        {
            return Cache.Get(GetKey(expression)) != null;
        }

        public bool Exists<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return Cache.Get(GetKey(expression)) != null;
        }

        public async Task<bool> ExistsAsync<TResult>(Expression<Func<TResult>> expression)
        {
            return await Cache.GetAsync(GetKey(expression)) != null;
        }

        public async Task<bool> ExistsAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return await Cache.GetAsync(GetKey(expression)) != null;
        }

        public TResult Get<TResult>(Expression<Func<TResult>> expression)
        {
            return Provider.Get<TResult>(GetKey(expression)).Content;
        }

        public TResult Get<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return Provider.Get<TResult>(GetKey(expression)).Content;
        }

        public async Task<TResult> GetAsync<TResult>(Expression<Func<TResult>> expression)
        {
            return (await Provider.GetAsync<TResult>(GetKey(expression))).Content;
        }

        public async Task<TResult> GetAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return (await Provider.GetAsync<TResult>(GetKey(expression))).Content;
        }

        public async Task<IEnumerable<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<TResult>>> expressions)
        {
            return await GetManyBaseAsync<TResult>(expressions.Select(GetKey).ToList());
        }

        public async Task<IEnumerable<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<Task<TResult>>>> expressions)
        {
            return await GetManyBaseAsync<TResult>(expressions.Select(GetKey).ToList());
        }

        private async Task<IEnumerable<TResult>> GetManyBaseAsync<TResult>(List<string> cacheKeyList)
        {
            var tasks = new List<Task<CacheResult<TResult>>>();
            cacheKeyList.ForEach(key => tasks.Add(Provider.GetAsync<TResult>(key)));
            var result = await Task.WhenAll(tasks);
            return result.Select(r => r.Content);
        }

        public void Set<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry)
        {
            if (expiry == null) throw new ArgumentNullException(nameof(expiry));
            Provider.Set(GetKey(expression), value, expiry);
        }

        public void Set<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry)
        {
            if (expiry == null) throw new ArgumentNullException(nameof(expiry));
            Provider.Set(GetKey(expression), value, expiry);
        }

        public async Task SetAsync<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry)
        {
            if (expiry == null) throw new ArgumentNullException(nameof(expiry));
            await Provider.SetAsync(GetKey(expression), value, expiry);
        }

        public async Task SetAsync<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry)
        {
            if (expiry == null) throw new ArgumentNullException(nameof(expiry));
            await Provider.SetAsync(GetKey(expression), value, expiry);
        }
    }
}

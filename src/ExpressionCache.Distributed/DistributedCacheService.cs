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
        public DistributedCacheService(IDistributedCache cache)
            : base(new DistributedCacheProvider(cache))
        {
        }

        protected IDistributedCache Cache => ((DistributedCacheProvider)Provider).Cache;

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
            await Cache.RemoveAsync(GetKey(expression)).ConfigureAwait(false);
        }

        public async Task RemoveAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            await Cache.RemoveAsync(GetKey(expression)).ConfigureAwait(false);
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
            return await Cache.GetAsync(GetKey(expression)).ConfigureAwait(false) != null;
        }

        public async Task<bool> ExistsAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return await Cache.GetAsync(GetKey(expression)).ConfigureAwait(false) != null;
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
            return (await Provider.GetAsync<TResult>(GetKey(expression)).ConfigureAwait(false)).Content;
        }

        public async Task<TResult> GetAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return (await Provider.GetAsync<TResult>(GetKey(expression)).ConfigureAwait(false)).Content;
        }

        public async Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<TResult>>> expressions)
        {
            return await GetManyBaseAsync<TResult>(expressions.Select(GetKey).ToList()).ConfigureAwait(false);
        }

        public async Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<Task<TResult>>>> expressions)
        {
            return await GetManyBaseAsync<TResult>(expressions.Select(GetKey).ToList()).ConfigureAwait(false);
        }

        public void Set<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry)
        {
            Provider.Set(GetKey(expression), value, expiry);
        }

        public void Set<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry)
        {
            Provider.Set(GetKey(expression), value, expiry);
        }

        public async Task SetAsync<TResult, TValue>(Expression<Func<TResult>> expression, TValue value, TimeSpan expiry)
        {
            await Provider.SetAsync(GetKey(expression), value, expiry).ConfigureAwait(false);
        }

        public async Task SetAsync<TResult, TValue>(Expression<Func<Task<TResult>>> expression, TValue value, TimeSpan expiry)
        {
            await Provider.SetAsync(GetKey(expression), value, expiry).ConfigureAwait(false);
        }

        private async Task<List<TResult>> GetManyBaseAsync<TResult>(List<string> cacheKeyList)
        {
            var tasks = new List<Task<CacheResult<TResult>>>();
            cacheKeyList.ForEach(key => tasks.Add(Provider.GetAsync<TResult>(key)));
            var result = await Task.WhenAll(tasks).ConfigureAwait(false);
            return result.Select(r => r.Content).ToList();
        }
    }
}

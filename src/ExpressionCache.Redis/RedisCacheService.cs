using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ExpressionCache.Core;
using StackExchange.Redis;

namespace ExpressionCache.Redis
{
    public class RedisCacheService : ExpressionCacheBase, IRedisCacheService
    {
        protected IDatabase Database => ((RedisCacheProvider) Provider).Database;

        public RedisCacheService(ConfigurationOptions options) : base(new RedisCacheProvider(options)) { }

        public void Remove<TResult>(Expression<Func<TResult>> expression)
        {
            Database.KeyDelete(GetKey(expression));
        }

        public void Remove<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            Database.KeyDelete(GetKey(expression));
        }

        public async Task RemoveAsync<TResult>(Expression<Func<TResult>> expression)
        {
            await Database.KeyDeleteAsync(GetKey(expression));
        }

        public async Task RemoveAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            await Database.KeyDeleteAsync(GetKey(expression));
        }

        public bool Exists<TResult>(Expression<Func<TResult>> expression)
        {
            return Database.KeyExists(GetKey(expression));
        }

        public bool Exists<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return Database.KeyExists(GetKey(expression));
        }

        public async Task<bool> ExistsAsync<TResult>(Expression<Func<TResult>> expression)
        {
            return await Database.KeyExistsAsync(GetKey(expression));
        }

        public async Task<bool> ExistsAsync<TResult>(Expression<Func<Task<TResult>>> expression)
        {
            return await Database.KeyExistsAsync(GetKey(expression));
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

        public async Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<TResult>>> expressions)
        {
            return await GetManyBaseAsync<TResult>(expressions.Select(GetKey).ToList());
        }

        public async Task<List<TResult>> GetManyAsync<TResult>(IEnumerable<Expression<Func<Task<TResult>>>> expressions)
        {
            return await GetManyBaseAsync<TResult>(expressions.Select(GetKey).ToList());
        }

        private async Task<List<TResult>> GetManyBaseAsync<TResult>(List<string> cacheKeyList)
        {
            var tasks = new List<Task<CacheResult<TResult>>>();
            cacheKeyList.ForEach(key => tasks.Add(Provider.GetAsync<TResult>(key)));
            var result = await Task.WhenAll(tasks);
            return result.Select(r => r.Content).ToList();
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

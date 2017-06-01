using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace ExpressionCache.Distributed.Tests
{
    public class DistributedCacheProviderTests
    {
        private readonly DistributedCacheProvider _distributedCacheProvider;

        public DistributedCacheProviderTests()
        {
            var cache = new MemoryDistributedCache(new MemoryCache(new MemoryCacheOptions()));
            _distributedCacheProvider = new DistributedCacheProvider(cache);
        }

        [Fact]
        public void Get()
        {
            
        }
    }
}

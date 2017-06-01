using System;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace ExpressionCache.Distributed.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDistributedExpressionCache_IServiceCollection_ShouldResolveService()
        {
            IServiceCollection services = new ServiceCollection();
            var cache = new MemoryDistributedCache(new MemoryCache(new MemoryCacheOptions()));

            services.AddSingleton<IDistributedCache>(cache);
            services.AddDistributedExpressionCache();

            var provider = services.BuildServiceProvider();
            var service = provider.GetService<IDistributedCacheService>();

            service.ShouldNotBeNull();
            service.ShouldBeOfType<DistributedCacheService>();
        }

        [Fact]
        public void AddDistributedExpressionCache_Null_ShouldThrowArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => ((IServiceCollection) null).AddDistributedExpressionCache());
        }
    }
}

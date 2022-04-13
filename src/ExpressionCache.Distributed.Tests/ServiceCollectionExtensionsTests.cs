using System;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace ExpressionCache.Distributed.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDistributedExpressionCache_IServiceCollection_ShouldResolveService()
        {
            IServiceCollection services = new ServiceCollection();

            var options = new OptionsWrapper<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
            var cache = new MemoryDistributedCache(options);

            services.AddSingleton<IDistributedCache>(cache);
            services.AddDistributedExpressionCache();

            var provider = services.BuildServiceProvider();
            var service = provider.GetService<IDistributedCacheService>();

            service.Should().NotBeNull();
            service.Should().BeOfType<DistributedCacheService>();
        }

        [Fact]
        public void AddDistributedExpressionCache_Null_ShouldThrowArgumentNullException()
        {
            Action act = () => ((IServiceCollection)null).AddDistributedExpressionCache();

            act.Should().Throw<ArgumentNullException>();
        }
    }
}

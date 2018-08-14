using System;
using ExpressionCache.Redis.Tests.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using StackExchange.Redis;

namespace ExpressionCache.Redis.Tests
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddDistributedExpressionCache_IServiceCollection_ShouldResolveService()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddRedisExpressionCache(new RedisFixture().Options);

            var provider = services.BuildServiceProvider();
            var service = provider.GetService<IRedisCacheService>();

            service.ShouldNotBeNull();
            service.ShouldBeOfType<RedisCacheService>();
        }

        [Fact]
        public void AddDistributedExpressionCache_Null_ShouldThrowArgumentNullException()
        {
            Should.Throw<ArgumentNullException>(() => ((IServiceCollection) null).AddRedisExpressionCache(new ConfigurationOptions()));
        }
    }
}

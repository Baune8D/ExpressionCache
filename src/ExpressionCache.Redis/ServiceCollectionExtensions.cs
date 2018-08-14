using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StackExchange.Redis;

namespace ExpressionCache.Redis
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisExpressionCache(this IServiceCollection services, ConfigurationOptions options)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAdd(ServiceDescriptor.Singleton(options));
            services.TryAdd(ServiceDescriptor.Singleton<IRedisCacheService, RedisCacheService>());

            return services;
        }
    }
}

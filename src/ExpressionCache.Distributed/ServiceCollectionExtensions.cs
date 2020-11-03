using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ExpressionCache.Distributed
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDistributedExpressionCache(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.TryAdd(ServiceDescriptor.Singleton<IDistributedCacheService, DistributedCacheService>());

            return services;
        }
    }
}

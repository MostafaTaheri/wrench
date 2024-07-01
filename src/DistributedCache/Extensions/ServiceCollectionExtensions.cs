using Microsoft.Extensions.DependencyInjection;
using Wrench.DistributedCache.Services;
using Wrench.DistributedCache.Interfaces;

namespace Wrench.DistributedCache.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWrenchDistributedRedisCache(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddScoped<IWrenchDistributedCache, WrenchDistributedWrapper>();
        serviceCollection.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString;
        });

        return serviceCollection;
    }
}

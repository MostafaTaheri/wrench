using Microsoft.Extensions.DependencyInjection;
using Wrench.Services.DistributedCache;
using Wrench.Interfaces.DistributedCache;

namespace Wrench.Extensions.DistributedCache;


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

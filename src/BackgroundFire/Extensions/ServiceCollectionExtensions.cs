
using System;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Wrench.Scheduler.Abstraction.Interfaces;
using Wrench.BackgroundFire.Services;

namespace Wrench.BackgroundFire.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundFire(this IServiceCollection serviceCollection, string connectionString)
    {
        serviceCollection.AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(2),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(2),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true
        }));
        
        serviceCollection.AddHangfireServer();

        return serviceCollection;
    }

    public static IServiceCollection AddBackgroundFireScheduler(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ISchedulerService, JobClientService>();

        return serviceCollection;
    }
}

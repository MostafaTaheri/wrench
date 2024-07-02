using Hangfire;
using Microsoft.AspNetCore.Builder;

namespace Wrench.BackgroundFire.Extensions;

public static class ApplicationBuilderExtension
{
    public static IApplicationBuilder UseBackgroundFire(this IApplicationBuilder builder)
    {
        builder.UseHangfireDashboard();
        builder.UseEndpoints(ep => ep.MapHangfireDashboard());

        return builder;
    }
}
using System;
using System.Linq.Expressions;
using Hangfire;
using Wrench.Scheduler.Abstraction.Interfaces;

namespace Wrench.BackgroundFire.Services;

public class JobClientService : ISchedulerService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public JobClientService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void Schedule(Expression<Action> action, double delayTimeInSecond)
    {
        _backgroundJobClient.Schedule(action, TimeSpan.FromSeconds(delayTimeInSecond));
    }

    public void Schedule<T>(Expression<Action> action, double delayTimeInSecond)
    {
        Schedule(action, delayTimeInSecond);
    }
}
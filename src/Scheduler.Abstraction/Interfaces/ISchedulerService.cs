using System;
using System.Linq.Expressions;

namespace Wrench.Scheduler.Abstraction.Interfaces;

public interface ISchedulerService
{
    void Schedule(Expression<Action> action, double delayTimeInSecond);
}
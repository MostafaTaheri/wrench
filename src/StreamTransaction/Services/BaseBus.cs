using System;
using System.Diagnostics;
using System.Threading.Tasks;
using EasyNetQ;
using Wrench.StreamTransaction.Interfaces;

namespace Wrench.StreamTransaction.Services;

internal class BaseBus : IBaseBus
{
    private readonly ActivitySource Activity = new(nameof(BaseBus));
    private static readonly string activityName = $"RabbitMq Send";
    private readonly global::EasyNetQ.IBus _bus;

    public BaseBus(global::EasyNetQ.IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T message)
    {
        StartActivity();
        await _bus.PubSub.PublishAsync(message: message);
    }

    public async Task FuturePublishAsync<T>(T message, TimeSpan time)
    {
        StartActivity();
        await _bus.Scheduler.FuturePublishAsync(message: message, delay: time);
    }

    public void Publish<T>(T message)
    {
        StartActivity();
        _bus.PubSub.Publish(message: message);
    }

    public void StartActivity()
    {
        var activity = Activity.StartActivity(name: activityName, kind: ActivityKind.Producer);
        AddActivityTags(activity);
    }

    private static void AddActivityTags(Activity? activity)
    {
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.rabbitmq.queue", "sample");
    }
}


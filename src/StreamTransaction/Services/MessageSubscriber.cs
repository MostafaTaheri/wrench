using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wrench.StreamTransaction.Contracts;

namespace Wrench.StreamTransaction.Services;

public abstract class MessageSubscriber<TMessage> where TMessage : BaseMessage
{
    private static readonly ActivitySource Activity = new(nameof(MessageSubscriber<TMessage>));
    private static readonly string activityName = $"RabbitMq Subscriber";
    protected readonly IServiceProvider _serviceProvider;

    public MessageSubscriber(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private static void AddActivityTags(Activity? activity)
    {
        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "queue");
        activity?.SetTag("messaging.rabbitmq.queue", "sample");
    }

    public abstract Task OnHandleAsync(TMessage message);

    public async Task HandleAsync(TMessage message)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var _logger = scope.ServiceProvider.GetService<ILogger<TMessage>>();
            var _serializedMessage = JsonConvert.SerializeObject(message);
            using var activity = Activity.StartActivity(name: activityName, kind: ActivityKind.Consumer);

            try
            {
                _logger?.LogInformation(
                    "RMQ message Loki Unique Id {lid}, Id {Id} received with body {body}",
                    message.LUniqueId,
                    message.UniqueId,
                    _serializedMessage
                );

                AddActivityTags(activity: activity);
                await OnHandleAsync(message: message);

                _logger?.LogInformation(
                    "RMQ message Loki Unique Id {lId}, Id {Id} handled with body {body}",
                    message.LUniqueId,
                    message.UniqueId,
                    _serializedMessage
                );
            }
            catch (Exception ex)
            {
                _logger?.LogError(
                    ex,
                    "RMQ Exception Loki Unique Id {lId}, Id {id} handled with body {body}, ex:",
                    message.LUniqueId,
                    message.UniqueId,
                    _serializedMessage
                );

                activity?.SetStatus(ActivityStatusCode.Error);
                activity?.AddEvent(new ActivityEvent(ex.Message));

                throw;
            }
        }
    }

}
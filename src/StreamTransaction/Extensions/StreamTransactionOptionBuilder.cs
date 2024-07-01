using Microsoft.Extensions.DependencyInjection;
using Wrench.StreamTransaction.Contracts;
using Wrench.StreamTransaction.Services;

namespace Wrench.StreamTransaction.Extensions;


public class StreamTransactionOptionBuilder
{
    private readonly IServiceCollection _services;

    public StreamTransactionOptionBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public void RegisterHandler<TMessage, THandler>()
            where THandler : MessageSubscriber<TMessage>
            where TMessage : BaseMessage
    {
        _services.AddSingleton(typeof(THandler));
        _services.AddSingleton(new HandlerMetadata(typeof(TMessage), typeof(THandler)));
    }
}
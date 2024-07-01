using System;
using System.Collections;
using System.Net.Http;
using EasyNetQ;
using EasyNetQ.Consumer;
using Polly.CircuitBreaker;
using Polly.Timeout;
using Wrench.StreamTransaction.Exceptions;

namespace Wrench.StreamTransaction.Services;

public class CustomConsumerErrorStrategy : DefaultConsumerErrorStrategy
{
    public CustomConsumerErrorStrategy(IPersistentConnection connection,
                                          ISerializer serializer,
                                          IConventions conventions,
                                          ITypeNameSerializer typeNameSerializer,
                                          IErrorMessageSerializer errorMessageSerializer,
                                          ConnectionConfiguration configuration) : base(connection,
                                                                                        serializer,
                                                                                        conventions,
                                                                                        typeNameSerializer,
                                                                                        errorMessageSerializer,
                                                                                        configuration)
    {

    }

    public override AckStrategy HandleConsumerCancelled(ConsumerExecutionContext context)
    {
        return base.HandleConsumerCancelled(context);
    }

    public override AckStrategy HandleConsumerError(ConsumerExecutionContext context, Exception exception)
    {
        switch (exception)
        {
            case HttpRequestException:
            case BrokenCircuitException:
            case TimeoutRejectedException:
            case TimeoutException:
            case NeedToRequeueException:
                return AckStrategies.NackWithRequeue;
            default:
                return base.HandleConsumerError(context, exception);
        }
    }

}
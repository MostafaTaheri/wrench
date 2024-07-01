using EasyNetQ;
using EasyNetQ.Consumer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using Wrench.StreamTransaction.Services;
using Wrench.StreamTransaction.Extensions;
using Wrench.StreamTransaction.Interfaces;

namespace Wrench.DistributedCache.Extensions;


public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStreamTransaction(this IServiceCollection serviceCollection, string connectionString, Action<StreamTransactionOptionBuilder> configureDelegate)
    {
        _ = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));
        var builder = new StreamTransactionOptionBuilder(serviceCollection);

        configureDelegate.Invoke(builder);

        serviceCollection.AddSingleton<IBaseBus>(sp =>
              {
                  var bus = RabbitHutch.CreateBus(connectionString,
                                          registerServices =>
                                          {
                                              registerServices.Register<ITypeNameSerializer, CustomTypeNameSerializer>();
                                              registerServices.Register<IConsumerErrorStrategy, CustomConsumerErrorStrategy>();
                                          });

                  var metadata = sp.GetServices<HandlerMetadata>();

                  metadata.ToList().ForEach(m =>
                            {
                                _ = m.MessageType ?? throw new ArgumentNullException(nameof(configureDelegate));
                                _ = m.MessageType.FullName ?? throw new ArgumentNullException(nameof(configureDelegate));

                                var handler = sp.GetRequiredService(m.HandlerType);
                                var del = m.CreateDelegate(handler);

                                var methodInf = typeof(PubSubExtensions)
                                              .GetMethods()
                                              .Where(m => m.Name == "Subscribe"
                                                          && m.GetParameters().Length == 4
                                                          && m.GetParameters()[2].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>))
                                              .FirstOrDefault();

                                _ = methodInf ?? throw new ArgumentNullException(nameof(methodInf));
                                var genericMethod = methodInf.MakeGenericMethod(m.MessageType);


                                genericMethod.Invoke(bus.PubSub,
                              new object[] { bus.PubSub, m.MessageType.FullName, del, CancellationToken.None });
                            });

                  return new BaseBus(bus);
              });

        return serviceCollection;
    }
}

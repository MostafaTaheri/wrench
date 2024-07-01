using System;
using System.Threading.Tasks;

namespace Wrench.StreamTransaction.Interfaces;

public interface IBaseBus
{

    Task PublishAsync<T>(T message);
    Task FuturePublishAsync<T>(T message, TimeSpan time);
    void Publish<T>(T message);
}
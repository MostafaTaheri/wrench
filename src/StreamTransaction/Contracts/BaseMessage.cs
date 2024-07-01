using System;

namespace Wrench.StreamTransaction.Contracts;

public abstract class BaseMessage
{
    public Guid LUniqueId { get; set; } = Guid.NewGuid();
    public int UniqueId { get; set; }
    public int TryCount { get; set; }
    public int DelayTimeInSecond { get; set; } = 2;
    public DateTime InitiateDateTime { get; set; } = DateTime.Now;
}
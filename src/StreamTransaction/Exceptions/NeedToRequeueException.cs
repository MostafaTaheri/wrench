using System;

namespace Wrench.StreamTransaction.Exceptions;

public class NeedToRequeueException : Exception
{
    public NeedToRequeueException() { }
}
using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Wrench.DistributedCache;

public sealed class WrenchDistributedWrapper : IWrenchDistributedCache
{
    private readonly IDistributedCache _distributedCache;

    public WrenchDistributedWrapper(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    
}
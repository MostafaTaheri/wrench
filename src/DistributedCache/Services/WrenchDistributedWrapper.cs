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

    public T? Get<T>(string key)
    {
        var data = _distributedCache.Get(key: key);

        return data != null ? JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data)) : default;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken token = default)
    {
        var data = await _distributedCache.GetAsync(key: key, token: token);

        return data != null ? JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data)) : default;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<T> getItemCallBack, TimeSpan absoluteExpiration, CancellationToken token = default)
    {
        var data = await _distributedCache.GetAsync(key: key, token: token);

        if (data is null)
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));

        var newData = getItemCallBack();

        if (newData != null)
        {

        }



    }

    public Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> getItemCallBack, TimeSpan absoluteExpiration, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public bool HasData(string key)
    {
        return _distributedCache.Get(key: key) != null ? true : false;
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Set(string key, object data, TimeSpan absoluteExpiration)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string key, object data, TimeSpan absoluteExpiration, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
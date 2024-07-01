using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Wrench.DistributedCache.Interfaces;

namespace Wrench.DistributedCache.Services;

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
        var data = await _distributedCache.GetAsync(key: key);

        if (data != null)
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));

        var newData = getItemCallBack();

        if (newData != null)
        {
            await SetAsync(key: key, data: newData, absoluteExpiration: absoluteExpiration, token: token);
            return newData;
        }

        return default;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> getItemCallBack, TimeSpan absoluteExpiration, CancellationToken token = default)
    {
        var data = await _distributedCache.GetAsync(key: key);

        if (data != null)
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(data));

        var newData = await getItemCallBack();

        if (newData != null)
        {
            await SetAsync(key: key, data: newData, absoluteExpiration: absoluteExpiration, token: token);
            return newData;
        }

        return default;
    }

    public bool HasData(string key)
    {
        return _distributedCache.Get(key: key) != null ? true : false;
    }

    public async Task RefreshAsync(string key, CancellationToken token = default)
    {
        await _distributedCache.RefreshAsync(key: key, token: token);
    }

    public void Set(string key, object data, TimeSpan absoluteExpiration)
    {
        var serializedData = JsonSerializer.SerializeToUtf8Bytes(data);
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration);

        _distributedCache.Set(key: key, value: serializedData, options: options);
    }

    public async Task SetAsync(string key, object data, TimeSpan absoluteExpiration, CancellationToken token = default)
    {
        var serializedData = JsonSerializer.SerializeToUtf8Bytes(data);
        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(absoluteExpiration);

        await _distributedCache.SetAsync(key: key, value: serializedData, options: options, token: token);
    }
}
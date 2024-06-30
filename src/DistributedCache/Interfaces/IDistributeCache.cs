using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributeCache;

public interface IDistributeCache
{
    public interface ILokiDistributeCache
    {
        T? Get<T>(string key);
        
        bool HasData(string key);

        Task<T?> GetAsync<T>(string key, CancellationToken token = default);

        Task<T?> GetOrSetAsync<T>(string key,
                                  Func<T> getItemCallBack,
                                  TimeSpan absoluteExpiration,
                                  CancellationToken token = default);

        void Set(string key, object data, TimeSpan absoluteExpiration);

        Task SetAsync(string key,
                      object data,
                      TimeSpan absoluteExpiration,
                      CancellationToken token = default);

        Task<T?> GetOrSetAsync<T>(string key,
                                  Func<Task<T>> getItemCallBack,
                                  TimeSpan absoluteExpiration,
                                  CancellationToken token = default);
                                  
        Task RefreshAsync(string key, CancellationToken token = default);

    }
}
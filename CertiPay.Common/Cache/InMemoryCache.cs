using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CertiPay.Common.Cache
{
    /// <summary>
    /// Basic implementation of a cache that stores items in memory
    /// utilizing the System.Runtime.Caching.MemoryCache instance
    /// </summary>
    public class InMemoryCache : ICache
    {
        public TimeSpan DefaultExpiration { get; set; }

        public InMemoryCache()
            : this(TimeSpan.FromDays(1))
        {
        }

        public InMemoryCache(TimeSpan defaultExpiration)
        {
            this.DefaultExpiration = defaultExpiration;
        }

        public Task<T> GetOrAdd<T>(string key, Func<T> factory)
        {
            return GetOrAdd(key, factory, DefaultExpiration);
        }

        public Task Remove(string key)
        {
            MemoryCache.Default.Remove(key);
            return Task.FromResult(0);
        }

        public Task<T> GetOrAdd<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            T val = default(T);

            if (false == TryGet<T>(key, out val))
            {
                val = factory.Invoke();

                Add(key, val, expiration);
            }

            return Task.FromResult(val);
        }

        public void Add<T>(String key, T val, TimeSpan expiration)
        {
            MemoryCache.Default.Add(key, val, DateTime.Now.Add(expiration));
        }

        public Boolean TryGet<T>(String key, out T val)
        {
            val = default(T);

            if (false == MemoryCache.Default.Contains(key))
            {
                return false;
            }

            val = (T)MemoryCache.Default.Get(key);

            return true;
        }
    }
}
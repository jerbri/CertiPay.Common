using CertiPay.Common.Cache;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace CertiPay.Common.Redis
{
    /// <summary>
    /// An instance of the ICache that stores cache data in a redis instance
    /// </summary>
    public class RedisCache : ICache
    {
        private readonly RedisConnection _connection;

        public RedisCache(RedisConnection connection)
        {
            this._connection = connection;
        }

        public TimeSpan DefaultExpiration { get { return TimeSpan.FromDays(1); } }

        public Task<T> GetOrAdd<T>(string key, Func<T> factory)
        {
            return GetOrAdd<T>(key, factory, DefaultExpiration);
        }

        public async Task<T> GetOrAdd<T>(string key, Func<T> factory, TimeSpan expiration)
        {
            T value = default(T);

            if (!TryGet(key, out value))
            {
                value = factory.Invoke();

                await Add(key, value);
            }

            return value;
        }

        public Boolean TryGet<T>(string key, out T value)
        {
            value = default(T);

            string json = _connection.GetClient().StringGet(key);

            if (String.IsNullOrWhiteSpace(json)) return false;

            value = JsonConvert.DeserializeObject<T>(json);

            return true;
        }

        public async Task Add<T>(string key, T value)
        {
            await _connection.GetClient().StringSetAsync(key, JsonConvert.SerializeObject(value));
        }

        public async Task Remove(string key)
        {
            await _connection.GetClient().KeyDeleteAsync(key, CommandFlags.FireAndForget);
        }
    }
}
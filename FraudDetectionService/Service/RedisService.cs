using StackExchange.Redis;
using System.Text.Json;

namespace FraudDetectionService.Service
{
    public interface IRedisService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> DeleteAsync(string key);
    }

    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;
        private readonly string _instanceName;

        public RedisService(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            _database = redis.GetDatabase();
            _instanceName = configuration["Redis:InstanceName"] ?? "";
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(_instanceName + key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var serialized = JsonSerializer.Serialize(value);
            
            var keyFull = _instanceName + key;
            RedisKey redisKey = new RedisKey(keyFull);
            await _database.StringSetAsync(redisKey, serialized, expiry.Value);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(_instanceName + key);
        }
    }
}

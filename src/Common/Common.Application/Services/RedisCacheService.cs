using System.Text.Json;
using Common.Application.Repositories;
using StackExchange.Redis;


namespace Common.Application.Services;

public class RedisCacheService(IConnectionMultiplexer mux) : ICacheService
{
    private readonly IDatabase _cache = mux.GetDatabase();

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
    {
        // todo need retry policy to prevent recurrent price fetches.
        var serialized = JsonSerializer.Serialize(value);

        await _cache.StringSetAsync(key, serialized, expiration);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        string? data = await _cache.StringGetAsync(key);
        return data == null ? default : JsonSerializer.Deserialize<T>(data);
    }
    
    public async Task RemoveAsync(string key)
    {
        await _cache.KeyDeleteAsync(key);
    }

    public async Task<double> Increment(string key)
    {
        return await _cache.StringIncrementAsync(key);
    }

    public async Task<double> Decrement(string key)
    {
        return await _cache.StringDecrementAsync(key);
    }
}
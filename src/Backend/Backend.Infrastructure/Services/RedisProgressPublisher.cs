using System;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Backend.Infrastructure.Services;

public class RedisProgressPublisher
{
    private readonly IDatabase _db;
    private const string KeyFormat = "plugin:progress:{0}";
    private readonly TimeSpan _ttl = TimeSpan.FromSeconds(30);

    public RedisProgressPublisher(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public Task PublishAsync(int pluginId, double progress)
    {
        var ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var key = string.Format(KeyFormat, pluginId);
        var value = $"{progress}:{ts}";
        // set value and TTL in one call
        return _db.StringSetAsync(key, value, _ttl);
    }
}
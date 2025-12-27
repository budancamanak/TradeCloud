using Common.Messaging.Events.PluginExecution;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

namespace Backend.Infrastructure.Services;

public class RedisPluginSignalPublisher
{
    private readonly IDatabase _db;
    private const string KeyFormat = "plugin:signal:{0}:{1}";
    private readonly TimeSpan _ttl = TimeSpan.FromSeconds(30);

    public RedisPluginSignalPublisher(IConnectionMultiplexer mux)
    {
        _db = mux.GetDatabase();
    }

    public Task PublishAsync(int pluginId, PluginSignalEvent @event)
    {
        var key = string.Format(KeyFormat, pluginId, @event.EventId);
        var value = JsonConvert.SerializeObject(@event);
        Log.Information("Publishing plugin signal to Redis with key {Key}", key);
        return _db.StringSetAsync(key, value, _ttl);
    }
}
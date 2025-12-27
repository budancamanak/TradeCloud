using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Common.Application.Queue;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using Common.Messaging.Events.PluginExecution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Backend.Infrastructure.Services;

public class PluginProgressService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnectionMultiplexer _mux;
    private readonly ILogger<PluginProgressService> _logger;
    private const string KeyPrefix = "plugin:progress:";

    public PluginProgressService(IServiceScopeFactory scopeFactory, IConnectionMultiplexer mux, ILogger<PluginProgressService> logger)
    {
        _scopeFactory = scopeFactory;
        _mux = mux;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var server = _mux.GetServer(_mux.GetEndPoints().First());
        var db = _mux.GetDatabase();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Use server.Keys with a pattern; for large scale use SCAN / batching
                foreach (var key in server.Keys(pattern: $"{KeyPrefix}*").Take(1000))
                {
                    var val = await db.StringGetAsync(key);
                    if (val.IsNullOrEmpty) continue;

                    var parts = ((string)val).Split(':');
                    if (parts.Length != 2) { await db.KeyExpireAsync(key, TimeSpan.FromSeconds(30)); continue; }

                    if (!double.TryParse(parts[0], out var progress)) { await db.KeyExpireAsync(key, TimeSpan.FromSeconds(30)); continue; }
                    if (!long.TryParse(parts[1], out var tsMillis)) { await db.KeyExpireAsync(key, TimeSpan.FromSeconds(30)); continue; }

                    var pluginIdStr = key.ToString().Substring(KeyPrefix.Length);
                    if (!int.TryParse(pluginIdStr, out var pluginId)) continue;

                    var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(tsMillis);

                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IPluginExecutionRepository>();

                    var updated = await repo.SetPluginProgressIfNewer(pluginId, progress, timestamp.UtcDateTime);

                    if (updated)
                    {
                        // remove key to avoid reprocessing; TTL acts as fallback
                        await db.KeyDeleteAsync(key);
                    }
                    else
                    {
                        // if not applied, keep/refresh TTL so flusher can skip stale items later
                        await db.KeyExpireAsync(key, TimeSpan.FromSeconds(30));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RedisProgressFlusher.");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}

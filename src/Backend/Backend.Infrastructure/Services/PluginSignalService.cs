using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Common.Messaging.Events.PluginExecution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Backend.Infrastructure.Services;

public class PluginSignalService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConnectionMultiplexer _mux;
    private readonly ILogger<PluginSignalService> _logger;
    private const string KeyPattern = "plugin:signal:*";

    public PluginSignalService(IServiceScopeFactory scopeFactory, IConnectionMultiplexer mux, ILogger<PluginSignalService> logger)
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
                foreach (var key in server.Keys(pattern: KeyPattern).Take(1000))
                {
                    var val = await db.StringGetAsync(key);
                    if (val.IsNullOrEmpty) continue;

                    PluginSignalEvent? signal;
                    try
                    {
                        signal = JsonConvert.DeserializeObject<PluginSignalEvent>(val!);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize signal from key {Key}", key);
                        await db.KeyDeleteAsync(key);
                        continue;
                    }

                    if (signal?.Signal == null)
                    {
                        _logger.LogWarning("Invalid signal data in key {Key}", key);
                        await db.KeyDeleteAsync(key);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IPluginOutputRepository>();
                    
                    try
                    {
                        var mr = await repo.AddAsync(new PluginOutput
                        {
                            CreatedDate = DateTime.UtcNow,
                            PluginId = signal.PluginId,
                            PluginSignal = signal.Signal.SignalType,
                            SignalDate = signal.Signal.SignalDate
                        });

                        if (mr.IsSuccess)
                        {
                            _logger.LogInformation("Signal saved for plugin {PluginId}: {SignalType}", 
                                signal.PluginId, signal.Signal.SignalType);
                            await db.KeyDeleteAsync(key);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to save signal: {Error}", mr.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Exception saving signal for plugin {PluginId}", signal.PluginId);
                        // Keep key for retry, TTL will clean up eventually
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PluginSignalService");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}

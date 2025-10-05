using System.Collections.Concurrent;
using Backend.Application.Abstraction.Repositories;
using Backend.Domain.Entities;
using Common.Application.Queue;
using Common.Core.Models;
using Common.Logging.Events.Backend;
using Common.Messaging.Events.PluginExecution;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Backend.Infrastructure.Services;

public class PluginSignalService(
    IPluginOutputRepository pluginOutputRepository,
    IPluginExecutionRepository pluginExecutionRepository,
    IBackgroundTaskQueue taskQueue,
    ILogger<PluginSignalService> logger) : QueuedHostedService(taskQueue, logger)
{
    protected override async Task ExecuteItem(IntegrationEvent workItem)
    {
        if (workItem is not PluginSignalEvent model) return;
        logger.LogInformation(ChartLogEvents.ExecutionSignal, "Consuming {Signal}", model.Signal.SignalType);
        var plugin = await pluginExecutionRepository.GetByIdAsync(model.PluginId);
        if (plugin == null) return;
        var mr = await pluginOutputRepository.AddAsync(new PluginOutput
        {
            PluginId = model.PluginId,
            PluginSignal = model.Signal.SignalType,
            CreatedDate = model.CreatedDate,
            SignalDate = model.Signal.SignalDate
        });
        logger.LogInformation(ChartLogEvents.ExecutionSignal,
            "Consumed PluginSignalEvent> Saving signal result for {PluginId}: {Signal} - Response: {Result}",
            model.PluginId,
            model.Signal, mr);
    }
}
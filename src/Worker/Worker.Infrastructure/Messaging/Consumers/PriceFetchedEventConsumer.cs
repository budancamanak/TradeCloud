using Common.Core.Enums;
using Common.Messaging.Events.PriceFetchEvents;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Worker.Application.Abstraction;

namespace Worker.Infrastructure.Messaging.Consumers;

/// <summary>
/// TODO Implement as order below:
/// <ul>
/// <li><b>TODO next "Backend" :</b></li>
/// <li><b>TODO list</b></li>
/// <li><b>TODO next:</b> </li>
/// </ul>
/// 
/// <b>TODO list</b>
/// <ul>
/// <li>we have the service to service communication over rabbitmq now. </li>
/// <li>market triggers worker service about a finished price fetch operation.</li>
/// <li>worker now should start a plugin with price data</li>
/// <li>worker should use <b>MediatR</b> to execute next operation.</li>
/// <li>next operation should make a grpc call to get price data and execute the plugin workload</li>
/// </ul>
/// <b>TODO next:</b> 
/// <ul>
/// <li>worker should remove plugin from its running queue when <see cref="PriceFetchedFailedIntegrationEvent"/> arrives</li>
/// <li>backend should listen for <see cref="PriceFetchedFailedIntegrationEvent"/> to update the plugin's status</li>
/// <li>plugin's status should be <see cref="PluginStatus.Failure"/> and error should indicate that price fetch failed somehow</li>
/// </ul>
///
/// <b>TODO next "Backend" :</b>
/// <ul>
/// <li>When plugin run api called, backend should check if there's a free slot to run the plugin over Grpc</li>
/// <li>if there's no slot, backend should update plugin status to <see cref="PluginStatus.Queued"/></li>
/// <li>if there's a slot, backend should trigger <b>Worker Service</b> over <b>RabbitMQ</b></li>
/// <li><b>Worker Service</b> will make <b>Grpc</b> call to <b>Market Service</b> for price data</li>
/// <li>if Market service returns price_data.length>0, it will execute</li>
/// <li>if Market Service returns empty array, it will wait for consumer to be triggered.</li>
/// </ul>
///
/// After clearing todos above, we should be able to create, start a plugin. Plugin should be able to use price information.
/// For missing price data, plugin should be able to wait for price fetch to finish.
///
/// <remarks>There can be multiple price fetch operation that are being done over same ticker for same timeframe for same time range</remarks>
/// <remarks>Make sure to fetch for a range of price just once. </remarks>
/// <remarks>Could use <see cref="Dictionary{TKey,TValue}"/> for time fetch windows to avoid re-fetch</remarks>
/// </summary>
/// <param name="logger"></param>
public class PriceFetchedEventConsumer(
    ILogger<PriceFetchedEventConsumer> logger,
    IMediator mediator,
    IPluginHost pluginHost)
    : IConsumer<PriceFetchedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PriceFetchedIntegrationEvent> context)
    {
        logger.LogWarning("CONSUMED >> Price fetch finished");
        var message = context.Message;
        logger.LogWarning("CONSUMED >> pluginId {}, eventId:{} @ {}", message.PluginId, message.EventId,
            message.CreatedDate);
        var request = pluginHost.GetRequestFor(message.PluginId);
        var mr = await mediator.Send(request);
        return;
    }
}
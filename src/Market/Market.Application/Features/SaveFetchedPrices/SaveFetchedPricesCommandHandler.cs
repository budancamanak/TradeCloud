using AutoMapper;
using Common.Core.DTOs;
using Common.Core.Models;
using Common.Logging.Events.Market;
using Common.Messaging.Abstraction;
using Common.Messaging.Events.PriceFetchEvents;
using FluentValidation;
using Market.Application.Abstraction.Services;
using Market.Application.Features.SaveFetchedPrices.Request;
using Market.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Market.Application.Features.SaveFetchedPrices;

public class SaveFetchedPricesCommandHandler(
    IPriceService priceService,
    IMapper mapper,
    IEventBus bus,
    IValidator<PriceFetchCompletedCommand> validator,
    ILogger<SaveFetchedPricesCommandHandler> logger)
    : IRequestHandler<PriceFetchCompletedCommand, MethodResponse>
{
    public async Task<MethodResponse> Handle(PriceFetchCompletedCommand request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        var dtos = mapper.Map<IList<PriceDto>, IList<Price>>(request.PriceInfo,
            opt =>
            {
                opt.Items["TickerId"] = request.TickerId;
                opt.Items["TimeFrame"] = request.TimeFrame;
            });
        logger.LogInformation(MarketLogEvents.PriceFetchCompleted,
            "Prices fetched for plugin[{PluginId}]. Saving.. Count:{Count}",
            request.PluginId, dtos.Count);
        var mr = await priceService.SaveMissingPricesAsync(dtos, request.CacheKey);
        logger.LogInformation(MarketLogEvents.PriceFetchCompleted,
            "Prices fetched for plugin[{PluginId}]. Saved.. Response: {Result}",
            request.PluginId, mr);
        if (mr.IsSuccess)
            await bus.PublishAsync(new PriceFetchedIntegrationEvent(request.PluginId));
        else
            await bus.PublishAsync(new PriceFetchedFailedIntegrationEvent(request.PluginId, mr.Message));

        // todo if successful, call kafka to notify worker to get price again
        // todo if fails, logError result call kafka to notify backend to update plugin status & notify worker to remove plugin from queue
        return mr;
    }
}
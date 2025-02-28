using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Core.Models;
using MediatR;

namespace Market.Application.Features.SaveFetchedPrices.Request;

public class PriceFetchCompletedCommand(
    IList<PriceDto> priceInfo,
    int pluginId,
    string cacheKey,
    int tickerId,
    Timeframe timeFrame)
    : IRequest<MethodResponse>
{
    public IList<PriceDto> PriceInfo => priceInfo;
    public int PluginId => pluginId;
    public string CacheKey => cacheKey;
    public int TickerId => tickerId;
    public Timeframe TimeFrame => timeFrame;

    public override string ToString()
    {
        return $"PluginId:{PluginId}, TickerId:{TickerId}, Timeframe:{TimeFrame}";
    }
}
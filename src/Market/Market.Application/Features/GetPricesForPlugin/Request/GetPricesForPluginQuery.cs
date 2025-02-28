using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Application.Services;
using Common.Core.DTOs;
using MediatR;

namespace Market.Application.Features.GetPricesForPlugin.Request;

public class GetPricesForPluginQuery : IRequest<IList<PriceDto>>
{
    private int _tickerId;

    // todo change pluginId to plugin identifier?
    // todo nope. same identifier plugin can run at the same time.
    // todo below pluginId is id of plugin execution
    private int _pluginId;
    private string _timeframe;
    private DateTime _startDate;
    private DateTime _endDate;

    public GetPricesForPluginQuery()
    {
    }

    public GetPricesForPluginQuery(int tickerId,
        int pluginId,
        string timeframe,
        DateTime startDate,
        DateTime endDate)
    {
        _pluginId = pluginId;
        _tickerId = tickerId;
        _timeframe = timeframe;
        _startDate = startDate;
        _endDate = endDate;
    }

    public string CacheKey => CacheKeyGenerator.PluginKey(StartDate, EndDate, TickerId, Timeframe);

    public int TickerId
    {
        get => _tickerId;
        set => _tickerId = value;
    }

    public int PluginId
    {
        get => _pluginId;
        set => _pluginId = value;
    }

    public Timeframe Timeframe
    {
        get => _timeframe.TimeFrameFromString();
        set => _timeframe = value.GetStringRepresentation();
    }

    public DateTime StartDate
    {
        get => _startDate.FitDateToTimeFrame(Timeframe.GetMilliseconds(), true);
        set => _startDate = value;
    }

    public DateTime EndDate
    {
        get => _endDate.FitDateToTimeFrame(Timeframe.GetMilliseconds(), false);
        set => _endDate = value;
    }


    public override string ToString()
    {
        return
            $"PluginId:{PluginId}, TickerId:{TickerId}, Timeframe:{Timeframe}, StartDate:{StartDate}, EndDate:{EndDate}";
    }
}
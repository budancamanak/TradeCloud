using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.Execution.CreateAnalysisExecution;

public class CreateAnalysisExecutionRequest : IRequest<MethodResponse>
{
    public CreateAnalysisExecutionRequest(string pluginIdentifier, string symbol, string timeframe, DateTime startDate,
        string paramSet)
    {
        PluginIdentifier = pluginIdentifier;
        Symbol = symbol;
        _timeframe = timeframe;
        _startDate = startDate;
        ParamSet = paramSet;
    }

    public string PluginIdentifier { get; set; }
    public string Symbol { get; set; }
    private string _timeframe { get; set; }
    private DateTime _startDate { get; set; }
    private DateTime? _endDate { get; set; }
    public string ParamSet { get; set; }
    public string TradingParams { get; set; }

    public Timeframe Timeframe
    {
        get => _timeframe.TimeFrameFromString();
        set => _timeframe = value.GetStringRepresentation();
    }

    public DateTime StartDate
    {
        get => _startDate.FitDateToTimeFrame(Timeframe.GetMilliseconds(), false);
        set => _startDate = value;
    }

    public DateTime EndDate
    {
        get => _endDate?.FitDateToTimeFrame(Timeframe.GetMilliseconds(), false) ?? DateTime.UtcNow;
        set => _endDate = value;
    }
}
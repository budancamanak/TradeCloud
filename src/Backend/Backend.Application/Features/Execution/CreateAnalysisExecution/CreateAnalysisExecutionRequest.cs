using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.Execution.CreateAnalysisExecution;

public class CreateAnalysisExecutionRequest : IRequest<MethodResponse>
{
    public string PluginIdentifier { get; set; }
    public string Symbol { get; set; }
    public Timeframe Timeframe { get; set; }
    public string ParamSet { get; set; }
    public string TradingParams { get; set; }
    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
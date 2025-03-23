using Common.Core.Models;
using Common.Messaging.Events.AnalysisExecution;
using MediatR;

namespace Worker.Application.Features.RunAnalysisRequested;

public class RunAnalysisRequest : IRequest<MethodResponse>
{
    public int ExecutionId { get; set; }
    public string Identifier { get; set; }
    public int Ticker { get; set; }
    public string Timeframe { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }
    public List<RunPluginInfo> PluginInfos { get; set; }
}
using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.Execution.RunPluginExecution;

public class RunPluginExecutionRequest : IRequest<MethodResponse>
{
    public int ExecutionId { get; set; }
    // public string Identifier { get; set; }
    // public int Ticker { get; set; }
    // public string Timeframe { get; set; }
    // public DateTime EndDate { get; set; }
    // public DateTime StartDate { get; set; }
}
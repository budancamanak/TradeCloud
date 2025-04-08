using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.Execution.StopAnalysisExecution;

public class StopAnalysisExecutionRequest : IRequest<MethodResponse>
{
    public int AnalysisExecutionId { get; set; }
}
using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.Execution.RunAnalysisExecution;

public class RunAnalysisExecutionRequest : IRequest<MethodResponse>
{
    public int ExecutionId { get; set; }
}
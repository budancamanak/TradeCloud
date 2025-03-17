using Common.Core.Models;
using MediatR;

namespace Backend.Application.Features.Execution.RunAnalysisExecution;

public class RunAnalysisExecutionRequestHandler : IRequestHandler<RunAnalysisExecutionRequest, MethodResponse>
{
    public Task<MethodResponse> Handle(RunAnalysisExecutionRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
using FluentValidation;

namespace Backend.Application.Features.Execution.RunAnalysisExecution;

public class RunAnalysisExecutionRequestValidator: AbstractValidator<RunAnalysisExecutionRequest>
{
    public RunAnalysisExecutionRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("RunAnalysisExecutionRequest can't be null");
        RuleFor(f => f.ExecutionId).NotNull().WithMessage("RunAnalysisExecutionRequest.ExecutionId can't be null")
            .GreaterThan(0).WithMessage("RunAnalysisExecutionRequest.ExecutionId must be positive");
    }
}
using FluentValidation;

namespace Backend.Application.Features.Execution.RunPluginExecution;

public class RunPluginExecutionRequestValidator : AbstractValidator<RunPluginExecutionRequest>
{
    public RunPluginExecutionRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("RunPluginExecutionRequest can't be null");
        RuleFor(f => f.ExecutionId).NotNull().WithMessage("RunPluginExecutionRequest.ExecutionId can't be null")
            .GreaterThan(0).WithMessage("RunPluginExecutionRequest.ExecutionId must be positive");
    }
}
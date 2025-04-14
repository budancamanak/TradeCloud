using FluentValidation;

namespace Backend.Application.Features.Execution.ListActivePlugins;

public class ListActivePluginsRequestValidator : AbstractValidator<ListActivePluginsRequest>
{
    public ListActivePluginsRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("ListActivePluginsRequest request can't be null");
        RuleFor(f => f.AnalysisExecutionId).GreaterThan(0)
            .WithMessage("ListActivePluginsRequest.ExecutionId can't be lower than 1");
    }
}
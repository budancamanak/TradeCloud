using FluentValidation;

namespace Backend.Application.Features.Execution.ListAvailablePlugins;

public class ListAvailablePluginsRequestValidator : AbstractValidator<ListAvailablePluginsRequest>
{
    public ListAvailablePluginsRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("List active plugins request can't be null");
    }
}
using Backend.Domain.Entities;
using FluentValidation;

namespace Backend.Application.Validators;

public class PluginOutputValidator : AbstractValidator<PluginOutput>
{
    public PluginOutputValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("PluginOutput can't be null");
        RuleFor(f => f.PluginId).NotNull().WithMessage("PluginOutput.Id can't be null")
            .GreaterThan(0).WithMessage("PluginOutput.Id is invalid");
        RuleFor(f => f.PluginSignal).NotNull().WithMessage("PluginOutput.Signal can't be null")
            .IsInEnum().WithMessage("PluginOutput.Signal is invalid");
        RuleFor(f => f.SignalDate).NotNull().WithMessage("PluginOutput.SignalDate can't be null")
            .NotEqual(default(DateTime)).WithMessage("PluginOutput.SignalDate is invalid");
    }
}
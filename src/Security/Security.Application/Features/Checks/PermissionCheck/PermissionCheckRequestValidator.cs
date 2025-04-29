using FluentValidation;

namespace Security.Application.Features.Checks.PermissionCheck;

public class PermissionCheckRequestValidator : BaseCheckRequestValidator<PermissionCheckRequest>
{
    public PermissionCheckRequestValidator() : base()
    {
        RuleFor(f => f.Permission)
            .NotNull().WithMessage("CheckRequest.Permission can't be null")
            .NotEmpty().WithMessage("CheckRequest.Permission can't be empty");
    }
}
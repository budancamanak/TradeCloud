using Common.Core.Models;
using FluentValidation;

namespace Security.Application.Features.Checks.PermissionCheck;

public class PermissionCheckRequestValidator : BaseCheckRequestValidator<PermissionCheckRequest, MethodResponse>
{
    public PermissionCheckRequestValidator() : base()
    {
        RuleFor(f => f.Permission)
            .NotNull().WithMessage("CheckRequest.Permission can't be null")
            .NotEmpty().WithMessage("CheckRequest.Permission can't be empty");
    }
}
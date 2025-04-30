using FluentValidation;

namespace Security.Application.Features.Checks;

public class BaseCheckRequestValidator<T, TV> : AbstractValidator<T>
    where T : BaseCheckRequest<TV>
{
    public BaseCheckRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("CheckRequest can't be null");
        RuleFor(f => f.Token)
            .NotNull().WithMessage("CheckRequest.Token can't be null")
            .NotEmpty().WithMessage("CheckRequest.Token can't be empty");
        RuleFor(f => f.ClientIp)
            .NotNull().WithMessage("CheckRequest.ClientIp can't be null")
            .NotEmpty().WithMessage("CheckRequest.ClientIp can't be empty");
    }
}
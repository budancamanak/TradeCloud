using FluentValidation;

namespace Security.Application.Features.User.LoginUser;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("LoginUserRequest can't be null");
        RuleFor(f => f.Email)
            .NotNull().WithMessage("LoginUserRequest.Email can't be null")
            .NotEmpty().WithMessage("LoginUserRequest.Email can't be empty")
            .MinimumLength(10).WithMessage("LoginUserRequest.Email can't shorter than 10")
            .MaximumLength(255).WithMessage("LoginUserRequest.Email can't longer than 255");
        RuleFor(f => f.Password)
            .NotNull().WithMessage("LoginUserRequest.Password can't be null")
            .NotEmpty().WithMessage("LoginUserRequest.Password can't be empty")
            .MinimumLength(8).WithMessage("LoginUserRequest.Password can't shorter than 8")
            .MaximumLength(25).WithMessage("LoginUserRequest.Password can't longer than 25");
        RuleFor(f => f.ClientIp)
            .NotNull().WithMessage("Failed to detect client IP address")
            .NotEmpty().WithMessage("Failed to detect client IP address");
    }
}
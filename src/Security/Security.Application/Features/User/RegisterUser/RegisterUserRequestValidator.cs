using FluentValidation;

namespace Security.Application.Features.User.RegisterUser;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("RegisterUserRequest can't be null");
        RuleFor(f => f.Email).NotEmpty().WithMessage("RegisterUserRequest.Email can't be null")
            .MinimumLength(10).WithMessage("RegisterUserRequest.Email's length can't be less than 10");
        RuleFor(f => f.Username).NotEmpty().WithMessage("RegisterUserRequest.Username can't be null")
            .MinimumLength(5).WithMessage("RegisterUserRequest.Username's length can't be less than 5");
        RuleFor(f => f.Password).NotEmpty().WithMessage("RegisterUserRequest.Email can't be null")
            .MinimumLength(8).WithMessage("RegisterUserRequest.Password's length can't be less than 8");
        RuleFor(f => f.PasswordConfirm).NotEmpty().WithMessage("RegisterUserRequest.Email can't be null")
            .MinimumLength(8).WithMessage("RegisterUserRequest.Password's length can't be less than 8");
        RuleFor(f => f.Password).Equal(f => f.PasswordConfirm)
            .WithMessage("RegisterUserRequest.Password not matching with confirm");
    }
}
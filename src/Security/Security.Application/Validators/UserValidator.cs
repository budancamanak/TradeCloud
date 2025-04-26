using FluentValidation;
using Security.Domain.Entities;

namespace Security.Application.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("User object can't be null");
        RuleFor(f => f.Email).NotEmpty().WithMessage("User.Email can't be null")
            .MinimumLength(10).WithMessage("User.Email's length can't be less than 10");
        RuleFor(f => f.Username).NotEmpty().WithMessage("User.Username can't be null")
            .MinimumLength(5).WithMessage("User.Username's length can't be less than 5");
        RuleFor(f => f.Password).NotEmpty().WithMessage("User.Email can't be null")
            .MinimumLength(8).WithMessage("User.Password's length can't be less than 8");
        RuleFor(f => f.Status).NotNull().WithMessage("User.Status can't be null");
    }
}
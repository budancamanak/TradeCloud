using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.User.RegisterUser;

public class RegisterUserRequest : IRequest<MethodResponse>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PasswordConfirm { get; set; }
}
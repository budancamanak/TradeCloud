using Common.Core.Models;
using MediatR;

namespace Security.Application.Features.User.LoginUser;

public class LoginUserRequest : IRequest<MethodResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ClientIp { get; set; }
}
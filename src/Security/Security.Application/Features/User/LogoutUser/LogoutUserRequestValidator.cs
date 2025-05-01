using Common.Core.Models;
using Security.Application.Features.Checks;

namespace Security.Application.Features.User.LogoutUser;

public class LogoutUserRequestValidator : BaseCheckRequestValidator<LogoutUserRequest, MethodResponse>
{
}
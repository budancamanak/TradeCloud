using Common.Core.Models;
using Common.Grpc;
using FluentValidation;
using MediatR;
using Security.Application.Abstraction.Services;
using Security.Application.Features.Checks;

namespace Security.Application.Features.User.LogoutUser;

public class LogoutUserRequestHandler(
    IValidator<LogoutUserRequest> validator,
    IUserService userService,
    ITokenService tokenService)
    : IRequestHandler<LogoutUserRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(LogoutUserRequest request,
        CancellationToken cancellationToken)
    {
        var validated = await validator.ValidateAsync(request, cancellationToken);
        if (validated is { IsValid: false })
        {
            return MethodResponse.Error(string.Join(" && ", validated.Errors));
        }

        var validateTokenResponse = await tokenService.ValidateToken(request.Token, request.ClientIp);
        if (!validateTokenResponse.IsValid)
        {
            return MethodResponse.Error(validateTokenResponse.UserId);
        }

        var mr = await userService.LogoutUser(request.Token);
        return mr;
    }
}
using Common.Core.Models;
using Common.Grpc;
using Common.Logging.Events.Security;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Services;
using Security.Application.Features.Checks;

namespace Security.Application.Features.User.LogoutUser;

public class LogoutUserRequestHandler(
    IValidator<LogoutUserRequest> validator,
    IUserService userService,
    ITokenService tokenService,
    ILogger<LogoutUserRequestHandler> logger)
    : IRequestHandler<LogoutUserRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(LogoutUserRequest request,
        CancellationToken cancellationToken)
    {
        try
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

            logger.LogWarning(UserLogEvents.LogoutUser, "Logging out user in with token: {Token}, IP: {ClientIp}",
                request.Token, request.ClientIp);
            var mr = await userService.LogoutUser(request.Token);
            if (!mr.IsSuccess)
                logger.LogWarning(UserLogEvents.LogoutUser,
                    "Failed to logout user in with token: {Token}, IP: {ClientIp}. Reason: {Reason}",
                    request.Token, request.ClientIp, mr.Message);
            return mr;
        }
        catch (Exception e)
        {
            logger.LogWarning(UserLogEvents.LogoutUser,
                "Failed to logout user in with token: {Token}, IP: {ClientIp}. Reason: {Reason}",
                request.Token, request.ClientIp, e.Message);
            return MethodResponse.Error(e.Message);
        }
    }
}
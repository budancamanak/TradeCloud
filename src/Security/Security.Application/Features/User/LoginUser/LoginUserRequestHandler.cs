using AutoMapper;
using Common.Core.Models;
using Common.Logging.Events.Security;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.User.LoginUser;

public class LoginUserRequestHandler(
    IValidator<LoginUserRequest> validator,
    IMapper mapper,
    IUserService userService,
    IUserRepository repository,
    ILogger<LoginUserRequestHandler> logger)
    : IRequestHandler<LoginUserRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var validated = await validator.ValidateAsync(request, cancellationToken);
            if (validated is { IsValid: false })
            {
                return MethodResponse.Error(string.Join(" && ", validated.Errors));
            }

            logger.LogWarning(UserLogEvents.LoginUser, "Logging user in with email: {Email}, IP: {ClientIp}",
                request.Email, request.ClientIp);
            var mr = await userService.LoginUser(request.Email, request.Password, request.ClientIp);
            if (!mr.IsSuccess)
                logger.LogWarning(UserLogEvents.LoginUser,
                    "Failed to log user in with email: {Email}, IP: {ClientIp}. Reason: {Reason}",
                    request.Email, request.ClientIp, mr.Message);
            return mr;
        }
        catch (Exception e)
        {
            logger.LogWarning(UserLogEvents.LoginUser,
                "Failed to log user in with email: {Email}, IP: {ClientIp}. Reason: {Reason}",
                request.Email, request.ClientIp, e.Message);
            return MethodResponse.Error(e.Message);
        }
    }
}
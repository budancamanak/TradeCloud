using AutoMapper;
using Common.Core.Enums;
using Common.Core.Models;
using Common.Security.Enums;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;

namespace Security.Application.Features.User.RegisterUser;

public class RegisterUserRequestHandler(
    IValidator<RegisterUserRequest> validator,
    IUserService userService,
    IUserRepository repository,
    IMapper mapper,
    ILogger<RegisterUserRequestHandler> logger)
    : IRequestHandler<RegisterUserRequest, MethodResponse>
{
    public async Task<MethodResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var validated = await validator.ValidateAsync(request, cancellationToken);
        if (validated is { IsValid: false })
        {
            return MethodResponse.Error(string.Join(" && ", validated.Errors));
        }
        // await validator.ValidateAndThrowAsync(request, cancellationToken);

        logger.LogWarning("Registering user with email: {Email} and username: {Username}", request.Email,
            request.Username);
        var mr = await userService.RegisterUser(new Domain.Entities.User
        {
            Email = request.Email,
            Password = request.Password,
            Username = request.Username,
            Status = Status.Active,
            CreatedDate = DateTime.UtcNow
        });
        if (!mr.IsSuccess) return mr;
        mr = await repository.AddRoleToUser(mr.Id, Roles.Viewer);
        if (mr.IsSuccess) return MethodResponse.Success("User registered successfully with Viewer role.");
        return MethodResponse.Success("User registered but failed to add Viewer role");
    }
}
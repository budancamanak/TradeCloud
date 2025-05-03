using Backend.Application.Abstraction.Services;
using Common.Core.Models;
using Common.Grpc;

namespace Backend.Infrastructure.Services;

public class UserGrpcClient(GrpcUserService.GrpcUserServiceClient grpcClient) : IUserGrpcClient
{
    public async Task<MethodResponse> LoginUserAsync(string email, string password)
    {
        var request = new GrpcUserLoginRequest
        {
            Email = email,
            Password = password
        };
        var mr = await grpcClient.LoginUserAsync(request);
        return new MethodResponse
        {
            Message = mr.Message,
            IsSuccess = mr.Success,
            Data = mr.Token
        };
    }

    public async Task<MethodResponse> RegisterUserAsync(string username, string email, string password,
        string passwordConfirm)
    {
        var request = new GrpcUserRegisterRequest
        {
            Email = email,
            Nickname = username,
            Password = password,
            PasswordConfirm = passwordConfirm
        };
        var mr = await grpcClient.RegisterUserAsync(request);
        return new MethodResponse
        {
            Message = mr.Message,
            IsSuccess = mr.Success
        };
    }

    public Task<MethodResponse> UserInfoAsync(string token, int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<MethodResponse> AddRoleToUserAsync(string token, int userId, int roleId)
    {
        var request = new GrpcAddRoleToUserRequest
        {
            RoleId = roleId,
            UserId = userId
        };
        var mr = await grpcClient.AddRoleToUserAsync(request);
        return new MethodResponse
        {
            Message = mr.Message,
            IsSuccess = mr.Success
        };
    }

    public async Task<MethodResponse> RemoveRoleFromUserAsync(string token, int userId, int roleId)
    {
        var request = new GrpcRemoveRoleFromUserRequest
        {
            RoleId = roleId,
            UserId = userId
        };
        var mr = await grpcClient.RemoveRoleFromUserAsync(request);
        return new MethodResponse
        {
            Message = mr.Message,
            IsSuccess = mr.Success
        };
    }

    public async Task<MethodResponse> AddPermissionToRoleAsync(string token, int roleId, int permissionId)
    {
        var request = new GrpcAddPermissionToRoleRequest
        {
            RoleId = roleId,
            PermissionId = permissionId
        };
        var mr = await grpcClient.AddPermissionToRoleAsync(request);
        return new MethodResponse
        {
            Message = mr.Message,
            IsSuccess = mr.Success
        };
    }

    public async Task<MethodResponse> RemovePermissionFromRoleAsync(string token, int roleId, int permissionId)
    {
        var request = new GrpcRemovePermissionFromRoleRequest
        {
            RoleId = roleId,
            PermissionId = permissionId
        };
        var mr = await grpcClient.RemovePermissionFromRoleAsync(request);
        return new MethodResponse
        {
            Message = mr.Message,
            IsSuccess = mr.Success
        };
    }
}
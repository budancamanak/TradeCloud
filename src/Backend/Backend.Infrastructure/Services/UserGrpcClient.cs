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

    public Task<MethodResponse> RegisterUserAsync(string username, string email, string password,
        string passwordConfirm)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> UserInfoAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> AddRoleToUserAsync(string token, int roleId)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> RemoveRoleFromUserAsync(string token, int roleId)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> AddPermissionToRoleAsync(string token, int roleId, int permissionId)
    {
        throw new NotImplementedException();
    }

    public Task<MethodResponse> RemovePermissionFromRoleAsync(string token, int roleId, int permissionId)
    {
        throw new NotImplementedException();
    }
}
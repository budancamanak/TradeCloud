﻿using Common.Grpc;
using Common.Security.Abstraction;

namespace Common.Security.Services;

public class SecurityGrpcClient(GrpcAuthController.GrpcAuthControllerClient grpcClient) : ISecurityGrpcClient
{
    public async Task<bool> HasPermissionAsync(string token, string permission)
    {
        var response = await grpcClient.CheckPermissionAsync(new CheckRequest() { Token = token, Value = permission });
        return response.Granted;
    }

    public async Task<bool> HasRoleAsync(string token, string role)
    {
        var response = await grpcClient.CheckPermissionAsync(new CheckRequest() { Token = token, Value = role });
        return response.Granted;
    }

    public async Task<bool> HasScopeAsync(string token, string scope)
    {
        var response = await grpcClient.CheckPermissionAsync(new CheckRequest() { Token = token, Value = scope });
        return response.Granted;
    }

    public async Task<bool> HasPolicyAsync(string token, string policy)
    {
        var response = await grpcClient.CheckPermissionAsync(new CheckRequest() { Token = token, Value = policy });
        return response.Granted;
    }
}
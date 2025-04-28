using Common.Web.Http;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace Common.Grpc.Interceptors;

public class AuthHeadersInterceptor(IHttpContextAccessor httpContextAccessor) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var metadata = new Metadata
        {
            { "HttpHeaderNames.AuthorizationXX", $"Bearer <JWT_TOKEN>" }
        };
        var ip = httpContextAccessor.GetClientIp();
        metadata.Add("ClientIP", ip);
        var userIdentity = httpContextAccessor.HttpContext?.User.Identity;
        if (userIdentity != null && userIdentity.IsAuthenticated)
        {
            // metadata.Add(httpContextAccessor.HttpContext.User, userIdentity.Name);
        }

        var callOption = context.Options.WithHeaders(metadata);
        context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, callOption);

        return base.AsyncUnaryCall(request, context, continuation);
    }
}
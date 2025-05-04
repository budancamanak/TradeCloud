using Common.Web.Http;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace Common.Security.Interceptors;

public class AuthHeadersInterceptor(IHttpContextAccessor httpContextAccessor) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var ip = httpContextAccessor.GetClientIp();
        var metadata = new Metadata
        {
            { "Token", $"Bearer " + httpContextAccessor.HttpContext?.Request.Headers["Authentication"] },
            { "ClientIP", ip }
        };
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
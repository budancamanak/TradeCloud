using System.Reflection;
using Common.Grpc;
using Worker.API.Grpc;

namespace Worker.API;

public static class DependencyInjection
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // services.AddExceptionHandler<GlobalExceptionHandler>();
        // services.AddProblemDetails();
        services.AddGrpcClient<GrpcPriceService.GrpcPriceServiceClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Market:GrpcHost"]);
        }).EnableCallContextPropagation();;
        services.AddGrpcClient<GrpcAuthService.GrpcAuthServiceClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Security:GrpcHost"]);
        }).EnableCallContextPropagation();;
    }

    public static void AddGrpcControllers(this WebApplication app)
    {
        app.MapGrpcService<GrpcWorkerController>();
        app.MapGrpcHealthChecksService().AllowAnonymous();;
    }
}
using System.Reflection;
using Common.Grpc;
using Common.Web;
using Security.API.Grpc;

namespace Security.API;

public static class DependencyInjection
{
    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        // services.AddGrpcClient<GrpcAuthController.GrpcAuthControllerClient>(cfg =>
        // {
        //     cfg.Address = new Uri(configuration["Market:GrpcHost"]);
        // });
    }

    public static void AddGrpcControllers(this WebApplication app)
    {
        app.MapGrpcService<GrpcSecurityController>();
        app.MapGrpcService<GrpcUserController>();
    }
}
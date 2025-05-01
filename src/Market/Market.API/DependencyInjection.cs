using System.Reflection;
using Market.API.Grpc;

namespace Market.API;

public static class DependencyInjection
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // services.AddExceptionHandler<GlobalExceptionHandler>();
        // services.AddProblemDetails();
    }

    public static void AddGrpcControllers(this WebApplication app)
    {
        app.MapGrpcService<GrpcPriceController>();
        app.MapGrpcService<GrpcTickerController>();
    }
}
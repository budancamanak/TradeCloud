using System.Reflection;
using Common.Grpc;
using Common.Security.Interceptors;

namespace Backend.API;

public static class DependencyInjection
{
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        // services.AddScoped(provider => new MapperConfiguration(cfg =>
        // {
        //     cfg.AddProfile(new AnalysisModelsMappingProfile());
        // }).CreateMapper());
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        // services.AddExceptionHandler<GlobalExceptionHandler>();
        // services.AddProblemDetails();
    }

    public static void AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<AuthHeadersInterceptor>();
        services.AddGrpcClient<GrpcTickerService.GrpcTickerServiceClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Market:GrpcHost"]);
        });
        services.AddGrpcClient<GrpcAvailablePluginsService.GrpcAvailablePluginsServiceClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Worker:GrpcHost"]);
        });
        Console.WriteLine("Connecting to security:" + configuration["Security:GrpcHost"]);
        services.AddGrpcClient<GrpcAuthService.GrpcAuthServiceClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Security:GrpcHost"]);
        }).AddInterceptor<AuthHeadersInterceptor>();
        services.AddGrpcClient<GrpcUserService.GrpcUserServiceClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Security:GrpcHost"]);
        }).AddInterceptor<AuthHeadersInterceptor>();
    }
}
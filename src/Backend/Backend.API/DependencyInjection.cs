using Common.Grpc;

namespace Backend.API;

public static class DependencyInjection
{
    public static void AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<GrpcTickerController.GrpcTickerControllerClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Market:GrpcHost"]);
        });
        services.AddGrpcClient<GrpcAvailablePluginsController.GrpcAvailablePluginsControllerClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Worker:GrpcHost"]);
        });
    }
}
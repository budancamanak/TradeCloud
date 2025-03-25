using System.Reflection;
using AutoMapper;
using Backend.API.Mappers;
using Common.Grpc;
using Common.Web;
using FluentValidation;

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
﻿using System.Reflection;
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
        services.AddGrpcClient<GrpcPriceController.GrpcPriceControllerClient>(cfg =>
        {
            cfg.Address = new Uri(configuration["Market:GrpcHost"]);
        });
    }

    public static void AddGrpcControllers(this WebApplication app)
    {
        app.MapGrpcService<WorkerGrpcController>();
    }
}
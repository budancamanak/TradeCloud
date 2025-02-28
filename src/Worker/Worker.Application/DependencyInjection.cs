using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Worker.Application;

public static class DependencyInjection
{
    public static void AddWorkerApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            // cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
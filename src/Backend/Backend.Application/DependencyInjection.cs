using System.Reflection;
using Backend.Application.Behaviors;
using Common.Web;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        // services.AddKeyedScoped<ICacheBuilder, MarketCacheBuilder>("market");
        // services.AddKeyedScoped<ICacheBuilder, TickerCacheBuilder>("ticker");
        // services.AddKeyedScoped<ICacheBuilder, ExchangeCacheBuilder>("exchange");

        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
        //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>)); 
    }
}
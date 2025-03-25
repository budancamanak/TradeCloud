using System.Reflection;
using AutoMapper;
using Backend.Application.Abstraction.Services;
using Backend.Application.Behaviors;
using Backend.Application.Features.Execution.CreateAnalysisExecution;
using Backend.Application.Mappers;
using Common.Web;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Application;

public static class DependencyInjection
{
    public static void AddApplicationServices(this IServiceCollection services)
    {        
        // services.AddScoped(provider => new MapperConfiguration(cfg =>
        // {
        //     cfg.AddProfile(new AnalysisExecutionMappingProfile());
        //     cfg.AddProfile(new CreateAnalysisExecutionRequestMappingProfile());
        // }).CreateMapper());
        // services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        // services.AddAutoMapper(typeof(AnalysisExecutionMappingProfile));
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        // services.AddScoped(provider => new MapperConfiguration(cfg =>
        // {
        //     cfg.AddProfile(new AnalysisExecutionMappingProfile());
        //     cfg.AddProfile(new PluginExecutionMappingProfile());
        //     cfg.AddProfile(new PluginOutputMappingProfile());
        //     cfg.AddProfile(new TrackListMappingProfile());
        // }).CreateMapper());
        // services.AddScoped(provider => new MapperConfiguration(cfg =>
        // {
        //     // cfg.AddProfile(new PluginExecutionMappingProfile(provider.GetService<ITickerService>()!,
        //     //     provider.GetService<IPluginService>()!));
        //     cfg.AddProfile(new AnalysisExecutionMappingProfile(provider.GetService<ITickerService>()!,
        //         provider.GetService<IPluginService>()!));
        //     // cfg.AddProfile(new PluginOutputMappingProfile(provider.GetService<IPluginService>()));
        //     // cfg.AddProfile(new TrackListMappingProfile(provider.GetService<ITickerService>()));
        // }).CreateMapper());

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
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Backend.Application.Mappers;
using Backend.Application.Validators;
using Backend.Domain.Entities;
using Backend.Infrastructure.Messaging.Consumers;
using Backend.Infrastructure.Repositories;
using Backend.Infrastructure.Services;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Messaging.Events;
using Common.Messaging.Events.PluginExecution;
using Common.RabbitMQ;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using MassTransit;

namespace Backend.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection services,
        IConfigurationManager configurationManager)
    {
        services.AddRabbitMqEventBus(configurationManager,
            consumerConfiguration: (config) =>
            {
                config.AddConsumer<PluginStatusEventConsumer>();
                config.AddConsumer<PluginSignalEventConsumer>();
                config.AddConsumer<PluginProgressEventConsumer>(cfg => { cfg.ConcurrentMessageLimit = 4; });
            },
            configure: (context, config) =>
            {
                config.Publish<IntegrationEvent>(f => f.Exclude = true);
                config.Message<RunPluginRequestedEvent>(f => f.SetEntityName("plugin.executions.exchange.run"));
                config.ReceiveEndpoint("plugin.executions.queue.status", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind("plugin.executions.exchange.status");
                    ep.ConfigureConsumer<PluginStatusEventConsumer>(context);
                });
                config.ReceiveEndpoint("plugin.executions.queue.progress", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind("plugin.executions.exchange.progress");
                    ep.ConfigureConsumer<PluginProgressEventConsumer>(context);
                });
                config.ReceiveEndpoint("plugin.executions.queue.signal", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind("plugin.executions.exchange.signal");
                    ep.ConfigureConsumer<PluginSignalEventConsumer>(context);
                });
            }
        );
        services.AddScoped<ITrackListRepository, TrackListRepository>();
        services.AddScoped<IPluginExecutionRepository, PluginExecutionRepository>();
        services.AddScoped<IPluginOutputRepository, PluginOutputRepository>();

        services.AddScoped<IValidator<PluginExecution>, PluginExecutionsValidator>();
        services.AddScoped<IValidator<TrackList>, TrackListValidator>();
        services.AddScoped<IValidator<PluginOutput>, PluginOutputValidator>();

        services.AddScoped<IPluginService, PluginService>();
        services.AddScoped<ITickerService, TickerService>();

        services.AddScoped(provider => new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new PluginExecutionMappingProfile(provider.GetService<ITickerService>()!,
                provider.GetService<IPluginService>()!));
            // cfg.AddProfile(new PluginOutputMappingProfile(provider.GetService<IPluginService>()));
            // cfg.AddProfile(new TrackListMappingProfile(provider.GetService<ITickerService>()));
        }).CreateMapper());


        //
        var multiplexer = ConnectionMultiplexer.Connect(configurationManager.GetConnectionString("Redis"));
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddTransient<ICacheService, RedisCacheService>();
        // serviceCollection.AddTransient<IPriceService, PriceService>();
        // serviceCollection.AddTransient<ITickerService, TickerService>();
        // serviceCollection.AddTransient<IPriceFetchCalculatorService, PriceFetchCalculatorService>();
        // serviceCollection.AddTransient<IPriceFetchJob, PriceFetchJob>();
    }
}
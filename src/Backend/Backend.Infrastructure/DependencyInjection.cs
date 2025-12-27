using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Backend.Application.Mappers;
using Backend.Application.Validators;
using Backend.Domain.Entities;
using Backend.Infrastructure.Messaging.Consumers;
using Backend.Infrastructure.Repositories;
using Backend.Infrastructure.Services;
using Common.Application.Queue;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Core.Models;
using Common.Messaging.Events.AnalysisExecution;
using Common.Messaging.Events.PluginExecution;
using Common.RabbitMQ;
using FluentValidation;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

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
                // config.AddConsumer<PluginComputationEventConsumer>();
                config.AddConsumer<PluginProgressEventConsumer>(cfg => { cfg.ConcurrentMessageLimit = 4; });
                // config.AddConsumer<AnalysisExecutionProgressEventConsumer>(cfg => { cfg.ConcurrentMessageLimit = 4; });
            },
            configure: (context, config) =>
            {
                config.Publish<IntegrationEvent>(f => f.Exclude = true);
                config.Message<RunAnalysisRequestedEvent>(f => f.SetEntityName("analysis.executions.exchange.run"));
                config.Message<StopAnalysisEvent>(f => f.SetEntityName("analysis.executions.exchange.stop"));
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
                // config.ReceiveEndpoint("analysis.executions.queue.progress", ep =>
                // {
                //     ep.ConfigureConsumeTopology = false;
                //     ep.Bind("analysis.executions.exchange.progress");
                //     ep.ConfigureConsumer<AnalysisExecutionProgressEventConsumer>(context);
                // });
                config.ReceiveEndpoint("plugin.executions.queue.signal", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind("plugin.executions.exchange.signal");
                    ep.ConfigureConsumer<PluginSignalEventConsumer>(context);
                });
                // config.ReceiveEndpoint("plugin.executions.computation", ep =>
                // {
                //     ep.ConfigureConsumeTopology = false;
                //     ep.Bind("plugin.executions.computation");
                //     ep.ConfigureConsumer<PluginComputationEventConsumer>(context);
                // });
            }
        );
        services.AddScoped<ITrackListRepository, TrackListRepository>();
        services.AddScoped<IPluginExecutionRepository, PluginExecutionRepository>();
        services.AddScoped<IAnalysisExecutionRepository, AnalysisExecutionRepository>();
        services.AddScoped<IPluginOutputRepository, PluginOutputRepository>();

        services.AddScoped<IValidator<PluginExecution>, PluginExecutionsValidator>();
        services.AddScoped<IValidator<TrackList>, TrackListValidator>();
        services.AddScoped<IValidator<PluginOutput>, PluginOutputValidator>();

        services.AddScoped<IPluginService, PluginService>();
        services.AddScoped<ITickerService, TickerService>();
        services.AddScoped<IPluginExecutionEngine, PluginExecutionEngine>();

        services.AddHostedService<PluginSignalService>();
        services.AddHostedService<PluginProgressService>();
        // services.AddSingleton<IBackgroundTaskQueue>(ctx =>
        // {
        //     if (!int.TryParse(configurationManager["QueueCapacity"], out var queueCapacity))
        //         queueCapacity = 100;
        //     return new BackgroundTaskQueue(queueCapacity);
        // });

        //
        var multiplexer = ConnectionMultiplexer.Connect(configurationManager.GetConnectionString("Redis"));
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddTransient<ICacheService, RedisCacheService>();
        // Register RedisProgressPublisher (singleton - thread-safe)
        services.AddSingleton<RedisProgressPublisher>();
        services.AddSingleton<RedisPluginSignalPublisher>();
        // serviceCollection.AddTransient<IPriceService, PriceService>();
        // serviceCollection.AddTransient<ITickerService, TickerService>();
        // serviceCollection.AddTransient<IPriceFetchCalculatorService, PriceFetchCalculatorService>();
        // serviceCollection.AddTransient<IPriceFetchJob, PriceFetchJob>();
    }
}
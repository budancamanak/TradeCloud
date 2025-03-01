using System.Reflection;
using Common.Application.Repositories;
using Common.Application.Services;
using Common.Messaging.Events;
using Common.Messaging.Events.PluginExecution;
using Common.Plugin.Abstraction;
using Common.RabbitMQ;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Worker.Application.Abstraction;
using Worker.Application.Services;
using Worker.Infrastructure.Messaging.Consumers;

namespace Worker.Infrastructure;

public static class DependencyInjection
{
    public static void AddWorkerServices(this IServiceCollection serviceCollection,
        IConfigurationManager configurationManager)
    {
        serviceCollection.AddRabbitMqEventBus(configurationManager,
            consumerConfiguration: (config) =>
            {
                config.AddConsumer<RunPluginRequestedEventConsumer>(configX =>
                {
                    configX.UseMessageRetry(f => f.Immediate(5));
                });
                config.AddConsumer<PriceFetchedEventConsumer>();
                config.AddConsumer<PriceFetchedFailedEventConsumer>();
                config.AddConsumer<RunPluginRequestedEventConsumer>();
            },
            configure: (context, config) =>
            {
                config.Publish<IntegrationEvent>(f => f.Exclude = true);
                config.Message<PluginProgressEvent>(f => f.SetEntityName("plugin.executions.exchange.progress"));
                config.Message<PluginSignalEvent>(f => f.SetEntityName("plugin.executions.exchange.signal"));
                config.Message<PluginStatusEvent>(f => f.SetEntityName("plugin.executions.exchange.status"));
                config.Message<PluginStartedEvent>(f => f.SetEntityName("plugin.executions.exchange.started"));
                config.Message<PluginFailedEvent>(f => f.SetEntityName("plugin.executions.exchange.failed"));
                config.Message<PluginSucceededEvent>(f => f.SetEntityName("plugin.executions.exchange.succeeded"));
                config.ReceiveEndpoint("price.fetch.queue", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind("price.fetch.exchange");
                    ep.ConfigureConsumer<PriceFetchedEventConsumer>(context);
                    ep.ConfigureConsumer<PriceFetchedFailedEventConsumer>(context);
                });
                config.ReceiveEndpoint("plugin.executions.queue.run", ep =>
                {
                    ep.ConfigureConsumeTopology = false;
                    ep.Bind("plugin.executions.exchange.run");
                    ep.ConfigureConsumer<RunPluginRequestedEventConsumer>(context);
                });
            }
        );
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
        serviceCollection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            // cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        serviceCollection.AddScoped<IPluginMessageBroker, RabbitMQPluginMessageBroker>();
        var multiplexer = ConnectionMultiplexer.Connect(configurationManager.GetConnectionString("Redis"));
        serviceCollection.AddSingleton<IConnectionMultiplexer>(multiplexer);
        serviceCollection.AddTransient<ICacheService, RedisCacheService>();
        serviceCollection.AddTransient<IReadOnlyCacheService, RedisCacheService>();
        // serviceCollection.AddScoped<IPluginHost, PluginHost>();
        serviceCollection.AddSingleton<IPluginHost, PluginHost>();
        serviceCollection.AddKeyedScoped<ICacheBuilder, WorkerCacheBuilder>("worker");
        serviceCollection.AddKeyedScoped<ICacheBuilder, AvailablePluginsCacheBuilder>("availablePlugins");

        serviceCollection.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(configurationManager.GetConnectionString("HangfireConnection"))));
        serviceCollection.AddHangfireServer((_, options) => { options.WorkerCount = Environment.ProcessorCount * 2; });
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute
            { Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Fail });
    }
}
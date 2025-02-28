using Common.Messaging.Abstraction;
using Common.Messaging.Events;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.RabbitMQ;

public static class DependencyInjection
{
    public static void AddRabbitMqEventBus(this IServiceCollection services, IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? consumerConfiguration = null,
        Action<IBusRegistrationContext, IRabbitMqBusFactoryConfigurator>? configure = null)
    {
        var rabbitMqHost = configuration["RabbitMQ:Host"];
        services.AddMassTransit(config =>
        {
            consumerConfiguration?.Invoke(config);
            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitMqHost ?? "rabbitmq://localhost");
                
                // cfg.Host("rabbitmq://localhost", h =>
                // {
                //     h.Username("guest");
                //     h.Password("guest");
                // });
                // cfg.ExchangeType = "topic";
                // cfg.SetExchangeArgument("name", "MarketExchange");
                configure?.Invoke(context, cfg);
            });
        });
        services.AddSingleton<IEventBus, RabbitMQBus>();
    }
}
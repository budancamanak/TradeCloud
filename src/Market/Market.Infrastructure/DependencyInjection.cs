using Common.Application.Repositories;
using Common.Application.Services;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Market.Application.Abstraction.Repositories;
using Market.Application.Abstraction.Services;
using Market.Application.Validators;
using Market.Domain.Entities;
using Market.Infrastructure.Repositories;
using Market.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace Market.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IServiceCollection serviceCollection,
        IConfigurationManager configurationManager)
    {
        serviceCollection.AddScoped<ITickerRepository, TickerRepository>();
        serviceCollection.AddScoped<IExchangeRepository, ExchangeRepository>();
        serviceCollection.AddScoped<IPriceRepository, PriceRepository>();

        serviceCollection.AddScoped<IValidator<Ticker>, TickerValidator>();
        serviceCollection.AddScoped<IValidator<Exchange>, ExchangeValidator>();
        serviceCollection.AddScoped<IValidator<Price>, PriceValidator>();

        var multiplexer = ConnectionMultiplexer.Connect(configurationManager.GetConnectionString("Redis"));
        serviceCollection.AddSingleton<IConnectionMultiplexer>(multiplexer);
        serviceCollection.AddTransient<ICacheService, RedisCacheService>();
        serviceCollection.AddTransient<IPriceService, PriceService>();
        serviceCollection.AddTransient<ITickerService, TickerService>();
        serviceCollection.AddTransient<IPriceFetchCalculatorService, PriceFetchCalculatorService>();
        serviceCollection.AddTransient<IPriceFetchJob, PriceFetchJob>();

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
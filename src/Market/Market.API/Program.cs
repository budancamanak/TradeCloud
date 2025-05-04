using Common.Application.Services;
using Common.Logging;
using Common.Messaging.Events;
using Common.Messaging.Events.PriceFetchEvents;
using Common.RabbitMQ;
using Hangfire;
using Market.API;
using Market.API.Models;
using Market.Application;
using Market.Infrastructure;
using Market.Infrastructure.Data;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MarketDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MarketDbConnection"),
        foptions =>
        {
            foptions.EnableRetryOnFailure(maxRetryCount: 4, maxRetryDelay: TimeSpan.FromSeconds(1),
                errorCodesToAdd: []);
        });
    // options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        options.ConfigureWarnings(waction =>
        {
            waction.Log(new EventId[]
            {
                CoreEventId.FirstWithoutOrderByAndFilterWarning,
                CoreEventId.RowLimitingOperationWithoutOrderByWarning,
                RelationalEventId.MultipleCollectionIncludeWarning
            });
        });
    }
});

builder.Services.AddRabbitMqEventBus(builder.Configuration, configure: (context, config) =>
{
    config.Publish<IntegrationEvent>(f => f.Exclude = true);
    config.Message<PriceFetchedIntegrationEvent>(f => f.SetEntityName("price.fetch.exchange"));
    config.Message<PriceFetchedFailedIntegrationEvent>(f => f.SetEntityName("price.fetch.exchange"));
});
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices();
// builder.Services.AddRabbitMqEventBus(builder.Configuration, (context, cfg) =>
// {
//     // cfg.UseSeriLog();
//     // cfg.ConfigureEndpoints(context);
//     // cfg.UseCircuitBreaker(cb =>
//     // {
//     //     cb.TrackingPeriod = TimeSpan.FromMinutes(1);  // Monitor for 1 minute
//     //     cb.ActiveThreshold = 5;
//     //     // cb.
//     //     // cb.SuccessThreshold = 5;  // Require 5 successful message deliveries before closing the breaker
//     //     // cb.FailureThreshold = 3;  // Open the breaker after 3 consecutive failures
//     //     // cb.Timeout = TimeSpan.FromSeconds(30);  // How long to wait before trying to close the circuit breaker
//     // });
//     // cfg.ReceiveEndpoint("price_worker_queue", e =>
//     // {
//     //     e.UseMessageRetry(r=>r.Interval(3,TimeSpan.FromSeconds(5)));
//     //     e.ConfigureConsumer<IntegrationEvent>(context);
//     // });
// });

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Duration | HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.RequestBody |
                            HttpLoggingFields.RequestQuery;
    options.CombineLogs = true;
});
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    // options.Interceptors.Add(new InterceptorRegistration());
});
builder.Services.AddGrpcHealthChecks()
    .AddCheck("Market.HealthCheck", () => HealthCheckResult.Healthy("Market.GRPC Service is UP"));
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5238,
        listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; });
    options.ListenAnyIP(5237, listenOptions =>
    {
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols
                .Http1AndHttp2;
    });
});
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    LogHelper.ConfigureLogger("market-api", builder.Configuration, context, configuration), true);

var app = builder.Build();
app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    logger.CreateLogger("ApplicationLifeCycle").LogInformation("MarketApi Started.. Building caches..");
    scope.ServiceProvider.GetRequiredKeyedService<ICacheBuilder>("market").BuildCacheAsync();
    // scope.Dispose();
});
app.Lifetime.ApplicationStopping.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    logger.CreateLogger("ApplicationLifeCycle").LogWarning("MarketApi Stopping..");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseExceptionHandler();
app.UseHangfireDashboard("/fetch_ops", new DashboardOptions
{
    DashboardTitle = "Price Fetch Jobs",
    Authorization = new[] { new HangFireAuthorizationFilter() }
});
app.MapControllers();
app.AddGrpcControllers();
app.MapHealthChecks("/health");
app.Run();
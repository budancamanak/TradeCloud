using Common.Application.Services;
using Common.Logging;
using Common.Security.Abstraction;
using Common.Security.Interceptors;
using Common.Security.Services;
using Hangfire;
using Worker.API;
using Worker.API.Models;
using Worker.Application;
using Worker.Infrastructure;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddWorkerServices(builder.Configuration);
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddWorkerApplicationServices();
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    // options.Interceptors.Add<ServerAuthInterceptor>();
});
builder.Services.AddGrpcHealthChecks()
    .AddCheck("Worker.HealthCheck", () => HealthCheckResult.Healthy("Worker.GRPC Service is UP"));
builder.Services.AddScoped<ISecurityGrpcClient, SecurityGrpcClient>(); // your implementation
/***
 * todo layout below !!
 * 1. Request C# files to build -> must not need extra packages. or need to make nuget call
 * 2. Request DLLs to run -> this is the way
 *
 *
 * 1. Request Python file & requirements.txt to run.
 *  0. Run pip install on requirements.txt
 *  1. Use PythonNet?
 *  2. Use .sh script?
 *
 */


/***
 * ### Think C# first ###
 * ........ Need a Common.Plugin project ..........
 * Plugins must have Execute() method
 * Plugins must take Logger, IEventbus as dependency -> nope
 * Plugins must be able to trigger rabbitmq -> will be redirected to trading
 *      -> do not give access to rabbitmq
 *      -> redirect events to rabbitmq through host
 * Plugins must be able to trigger logging -> will be redirected to elk stack
 *
 * ......... Need a Worker.Host project ...........
 * . Will list DLLs in plugins folder
 * . Will load plugins foreach DLL found
 * . Will set dependencies for each DLL found
 * Will queue plugin -> hangfire
 * Will trigger rabbitmq -> update pluginExecution state in backend
 *
 */

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5163,
        listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; });

    options.ListenAnyIP(5162,
        listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1; });
});
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Duration | HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.RequestBody |
                            HttpLoggingFields.RequestQuery;
    options.CombineLogs = true;
});
builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    LogHelper.ConfigureLogger("worker-api", builder.Configuration, context, configuration), true);

var app = builder.Build();
app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    logger.CreateLogger("WorkerApplicationLifeCycle").LogInformation("WorkerApi Started.. Building caches..");
    scope.ServiceProvider.GetRequiredKeyedService<ICacheBuilder>("worker").BuildCacheAsync();
    // scope.Dispose();
});
app.Lifetime.ApplicationStopping.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    logger.CreateLogger("WorkerApplicationLifeCycle").LogWarning("WorkerApi Stopping..");
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);
app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHangfireDashboard("/plugins", new DashboardOptions
{
    DashboardTitle = "Plugin Execution Jobs",
    Authorization = new[] { new HangFireAuthorizationFilter() }
});
app.MapControllers();
app.AddGrpcControllers();

app.Run();
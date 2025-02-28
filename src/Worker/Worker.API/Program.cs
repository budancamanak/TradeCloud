using Common.Application.Services;
using Hangfire;
using Worker.API;
using Worker.Application;
using Worker.Infrastructure;

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
    // options.Interceptors.Add(new InterceptorRegistration());
});
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
    options.ListenLocalhost(5163,
        listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; });

    // HTTP/1.1 for other endpoints (e.g., MVC or REST)
    options.ListenLocalhost(5162, listenOptions =>
    {
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols
                .Http1AndHttp2; // Allow both HTTP/1.1 and HTTP/2, but prefer HTTP/1.1.
    });
    // options.ConfigureEndpointDefaults(listenOptions =>
    // {
    //     listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    // });
});

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHangfireDashboard("/plugins", new DashboardOptions { DashboardTitle = "Plugin Execution Jobs" });
app.MapControllers();
app.AddGrpcControllers();

app.Run();
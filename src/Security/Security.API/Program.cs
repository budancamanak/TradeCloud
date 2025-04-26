using Common.Logging;
using Microsoft.EntityFrameworkCore;
using Security.API;
using Security.Application;
using Security.Infrastructure;
using Security.Infrastructure.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<SecurityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDbConnection"))
);

builder.Services.AddSecurityServices(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    // options.Interceptors.Add(new InterceptorRegistration());
});


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5501,
        listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; });

    options.ListenAnyIP(5500, listenOptions =>
    {
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols
                .Http1AndHttp2;
    });
});
builder.Host.UseSerilog((context, configuration) =>
    LogHelper.ConfigureLogger("security-api", builder.Configuration, context, configuration), true);

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() =>
{
    var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    logger.CreateLogger("SecurityApplicationLifeCycle").LogInformation("SecurityApi Started.. Building caches..");
    // scope.ServiceProvider.GetRequiredKeyedService<ICacheBuilder>("worker").BuildCacheAsync();
    // scope.Dispose();
});
app.Lifetime.ApplicationStopping.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
    logger.CreateLogger("SecurityApplicationLifeCycle").LogWarning("SecurityApi Stopping..");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.AddGrpcControllers();

app.Run();
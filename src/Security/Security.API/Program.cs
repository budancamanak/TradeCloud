using Common.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("SecurityDbConnection"),
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

builder.Services.AddApplicationServices();
builder.Services.AddApiServices(builder.Configuration);
builder.Services.AddSecurityServices(builder.Configuration);
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    // options.Interceptors.Add(new InterceptorRegistration());
});
builder.Services.AddGrpcHealthChecks()
    .AddCheck("Security.HealthCheck", () => HealthCheckResult.Healthy("Security.GRPC Service is UP"));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSecurityAuthentication(builder.Configuration);
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
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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

// app.UseAuthentication();
// app.UseAuthorization();
app.UseHttpsRedirection();
app.AddGrpcControllers();

app.Run();
using Backend.API;
using Backend.Application;
using Backend.Application.Abstraction.Services;
using Backend.Infrastructure;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Services;
using Common.Logging;
using Common.Security.Abstraction;
using Common.Security.Filters;
using Common.Security.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddRabbitMqEventBus(builder.Configuration);
builder.Services.AddDbContext<BackendDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("BackendDbConnection"),
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
    }
);


builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ISecurityGrpcClient, SecurityGrpcClient>(); // your implementation
builder.Services.AddScoped<IUserGrpcClient, UserGrpcClient>(); // your implementation
builder.Services.AddScoped<PermissionAuthorizationFilter>();

builder.Services.AddControllers(options => { options.Filters.Add<PermissionAuthorizationFilter>(); });
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(o =>
    {
        // var validator = new JwtTokenValidator(configuration);
        // o.UseSecurityTokenValidators = true;
        // o.SecurityTokenValidators.Add(validator);
        o.MapInboundClaims = false;
    });
builder.Services.AddAuthorization();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8081,
        listenOptions => { listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2; });

    options.ListenAnyIP(8080, listenOptions =>
    {
        listenOptions.Protocols =
            Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols
                .Http1AndHttp2;
    });
});
builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Duration | HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.RequestBody |
                            HttpLoggingFields.RequestQuery;
    options.CombineLogs = true;
});
// builder.Services.AddSerilog((provider, configuration) =>
//     LogHelper.ConfigureLogger("backend-api", builder.Configuration, provider, configuration));

builder.Logging.ClearProviders();
builder.Host.UseSerilog((context, configuration) =>
    LogHelper.ConfigureLogger("backend-api", builder.Configuration, context, configuration), true);

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging(opts => opts.EnrichDiagnosticContext = LogHelper.EnrichFromRequest);

app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();

app.MapControllers();
app.MapHealthChecks("/health");
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                       ForwardedHeaders.XForwardedProto
});
app.Run();
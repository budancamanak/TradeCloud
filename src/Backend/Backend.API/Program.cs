using Backend.API;
using Backend.Application;
using Backend.Infrastructure;
using Backend.Infrastructure.Data;
using Common.Logging;
using Common.Security.Abstraction;
using Common.Security.Filters;
using Common.Security.Services;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddRabbitMqEventBus(builder.Configuration);
builder.Services.AddDbContext<BackendDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BackendDbConnection"))
);

builder.Services.AddGrpcClients(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApiServices();
builder.Services.AddScoped<PermissionAuthorizationFilter>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<ISecurityGrpcClient, SecurityGrpcClient>(); // your implementation

builder.Services.AddControllers(options => { options.Filters.Add<PermissionAuthorizationFilter>(); });

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

app.UseAuthorization();
app.UseExceptionHandler();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
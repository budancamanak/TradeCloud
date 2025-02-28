using Backend.API;
using Backend.Application;
using Backend.Infrastructure;
using Backend.Infrastructure.Data;
using Common.Grpc;
using Common.RabbitMQ;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.Duration | HttpLoggingFields.RequestMethod |
                            HttpLoggingFields.RequestPath |
                            HttpLoggingFields.ResponseStatusCode | HttpLoggingFields.RequestBody |
                            HttpLoggingFields.RequestQuery;
    options.CombineLogs = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpLogging();
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseExceptionHandler();

app.MapControllers();

app.Run();
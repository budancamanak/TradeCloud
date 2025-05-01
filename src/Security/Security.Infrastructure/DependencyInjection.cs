using System.Reflection;
using Common.Application.Repositories;
using Common.Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Security.Application.Abstraction.Repositories;
using Security.Application.Abstraction.Services;
using Security.Infrastructure.Repositories;
using Security.Infrastructure.Services;
using StackExchange.Redis;

namespace Security.Infrastructure;

public static class DependencyInjection
{
    public static void AddSecurityServices(this IServiceCollection serviceCollection,
        IConfigurationManager configurationManager)
    {
        serviceCollection.AddAutoMapper(Assembly.GetExecutingAssembly());
        serviceCollection.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            // cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        var multiplexer = ConnectionMultiplexer.Connect(configurationManager.GetConnectionString("Redis"));
        serviceCollection.AddSingleton<IConnectionMultiplexer>(multiplexer);
        serviceCollection.AddTransient<ICacheService, RedisCacheService>();
        serviceCollection.AddTransient<IReadOnlyCacheService, RedisCacheService>();
        serviceCollection.AddTransient<IUserService, UserService>();
        serviceCollection.AddTransient<ITokenService, TokenService>();

        serviceCollection.AddScoped<IUserRepository, UserRepository>();
        serviceCollection.AddScoped<IRoleRepository, RoleRepository>();

        // services.AddScoped<IValidator<PluginExecution>, PluginExecutionsValidator>();
        // services.AddScoped<IValidator<TrackList>, TrackListValidator>();
        // services.AddScoped<IValidator<PluginOutput>, PluginOutputValidator>();
    }

    public static void AddSecurityAuthentication(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.AddAuthentication(options =>
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
        services.AddAuthorization();
    }
}
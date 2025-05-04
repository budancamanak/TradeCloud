using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.AspNetCore;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace Common.Logging;

public static class LogHelper
{
    public static void ConfigureLogger(string service, ConfigurationManager builderConfiguration,
        HostBuilderContext context,
        LoggerConfiguration loggerConfiguration)
    {
        // builderConfiguration.GetSection("Logging").GetSection("LogLevel")["Microsoft.AspNetCore"]

        var env = context.HostingEnvironment;
        loggerConfiguration.MinimumLevel.Information()
            .Enrich.FromLogContext()
            .Enrich.WithClientIp()
            .Enrich.WithMachineName()
            .Enrich.WithProperty("ApplicationName", env.ApplicationName)
            .Enrich.WithProperty("Service", service)
            .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
            .WriteTo.Console();

        var logConfigs = builderConfiguration.GetSection("Logging").GetSection("LogLevel").AsEnumerable().ToList();
        foreach (var kvp in logConfigs)
        {
            if (string.IsNullOrWhiteSpace(kvp.Key)) continue;
            if (string.IsNullOrWhiteSpace(kvp.Value)) continue;

            loggerConfiguration.MinimumLevel.Override(kvp.Key.Replace("Logging:LogLevel:", ""), kvp.Value.ToLogLevel());
        }
        
        var elasticUrl = context.Configuration.GetValue<string>("Elastic:Host");
        if (!string.IsNullOrEmpty(elasticUrl))
        {
            loggerConfiguration.WriteTo.Elasticsearch(
                new ElasticsearchSinkOptions(new Uri(elasticUrl))
                {
                    TypeName = null,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                    CustomFormatter = new ElasticsearchJsonFormatter(),
                    OverwriteTemplate = true,
                    DetectElasticsearchVersion = true,
                    IndexFormat =
                        $"{service}-logs-{env.EnvironmentName?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                    AutoRegisterTemplate = true,
                    NumberOfShards = 2,
                    NumberOfReplicas = 1
                });
        }
    }

    public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
    {
        var request = httpContext.Request;
        
        // Set all the common properties available for every request
        diagnosticContext.Set("Host", request.Host);
        diagnosticContext.Set("Protocol", request.Protocol);
        diagnosticContext.Set("Scheme", request.Scheme);
        diagnosticContext.Set("ClientIP", httpContext?.Connection.RemoteIpAddress?.ToString());
        diagnosticContext.Set("UserAgent", httpContext?.Request.Headers["User-Agent"].FirstOrDefault());
        // Only set it if available. You're not sending sensitive data in a querystring right?!
        if (request.QueryString.HasValue)
        {
            diagnosticContext.Set("QueryString", request.QueryString.Value);
        }
        
        // Set the content-type of the Response at this point
        diagnosticContext.Set("ContentType", httpContext.Response.ContentType);
        
        // Retrieve the IEndpointFeature selected for the request
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is object) // endpoint != null
        {
            diagnosticContext.Set("EndpointName", endpoint.DisplayName);
        }
    }
}
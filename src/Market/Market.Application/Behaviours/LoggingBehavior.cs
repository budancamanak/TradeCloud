using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Market.Application.Behaviours;

public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    private const int MaxResponseLengthToLog = 110;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid(); 
        var requestJson = JsonConvert.SerializeObject(request); 
        logger.LogInformation("Handling request {CorrelationID}: {Request}", request,
            requestJson[..Math.Min(MaxResponseLengthToLog, requestJson.Length)]);

        var response = await next();
        
        var responseJson = JsonConvert.SerializeObject(response);
        logger.LogInformation("Response for {Correlation}: {Response}", correlationId,
            responseJson[..Math.Min(MaxResponseLengthToLog, responseJson.Length)]);


        return response;
    }
}
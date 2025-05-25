using Ardalis.GuardClauses;
using AutoMapper;
using Backend.Application.Abstraction.Repositories;
using Backend.Application.Abstraction.Services;
using Common.Application.Repositories;
using Common.Core.DTOs;
using Common.Core.Enums;
using Common.Grpc;
using Common.Logging.Events.Backend;
using FluentValidation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Backend.Application.Features.Chart.ExecutionPrices;

public class GetExecutionPricesRequestHandler(
    IValidator<GetExecutionPricesRequest> validator,
    ICacheService cache,
    IAnalysisExecutionRepository executionRepository,
    ITickerService tickerService,
    IMapper mapper,
    ILogger<GetExecutionPricesRequestHandler> logger,
    GrpcPriceService.GrpcPriceServiceClient client)
    : IRequestHandler<GetExecutionPricesRequest, List<PriceDto>>
{
    public async Task<List<PriceDto>> Handle(GetExecutionPricesRequest request, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);
        logger.LogInformation(ChartLogEvents.ExecutionPrices,
            "Getting execution price information : {AnalysisExecutionId}", request.ExecutionId);
        var execution = await executionRepository.GetByIdAsync(request.ExecutionId);
        Guard.Against.Null(execution);
        var ticker = await tickerService.GetTickerWithId(execution.TickerId);
        Guard.Against.Null(ticker);
        var grpcRequest = new GrpcGetTickerPricesRequest
        {
            Ticker = ticker.Id,
            Timeframe = execution.Timeframe.GetStringRepresentation(),
            EndDate = execution.EndDate.ToTimestamp(),
            StartDate = execution.StartDate.ToTimestamp()
        };

        var mr = await client.GetTickerPricesAsync(grpcRequest, cancellationToken: cancellationToken);
        return mapper.Map<List<PriceDto>>(mr.Prices);
    }
}
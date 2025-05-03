using Ardalis.GuardClauses;
using Common.Grpc;
using Common.Security.Attributes;
using Common.Security.Enums;
using Common.Web.Exceptions;
using Google.Protobuf.Collections;
using Grpc.Core;
using Market.Application.Abstraction.Repositories;

namespace Market.API.Grpc;

public class GrpcTickerController(ITickerRepository repository, ILogger<GrpcTickerController> logger)
    : Common.Grpc.GrpcTickerService.GrpcTickerServiceBase
{
    [HasPermission(Permissions.Enum.ViewMarketData)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcAvailableTickersResponse> GetAvailableTickers(GrpcGetAvailableTickersRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("Getting available ticker information");
        var items = await repository.GetAllAsync();
        var rep = new RepeatedField<GrpcTickerResponse>();
        foreach (var tick in items)
        {
            rep.Add(new GrpcTickerResponse
            {
                Id = tick.Id,
                Name = tick.Name,
                Symbol = tick.Symbol,
                DecimalPoint = tick.DecimalPoint,
                ExchangeName = tick.Exchange.Name
            });
        }

        var result = new GrpcAvailableTickersResponse
        {
            Tickers = { rep }
        };
        return result;
    }

    [HasPermission(Permissions.Enum.ViewMarketData)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcTickerResponse> GetTickerWithId(GrpcGetTickerWithIdRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("Getting available ticker information with id:{tickerId}", request.TickerId);
        var item = await repository.GetByIdAsync(request.TickerId);
        Guard.Against.Null(item, message: "Couldn't find ticker");
        // todo need mapper here
        return new GrpcTickerResponse
        {
            Id = item.Id,
            Name = item.Name,
            Symbol = item.Symbol,
            DecimalPoint = item.DecimalPoint,
            ExchangeName = item.Exchange.Name
        };
    }

    [HasPermission(Permissions.Enum.ViewMarketData)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcTickerResponse> GetTickerWithSymbol(GrpcGetTickerWithSymbolRequest request,
        ServerCallContext context)
    {
        logger.LogInformation("Getting available ticker information with symbol:{symbol}", request.Symbol);
        var item = await repository.GetBySymbolAsync(request.Symbol);
        Guard.Against.Null(item, message: "Couldn't find ticker",
            exceptionCreator: () => new RequestValidationException("Couldn't find ticker"));
        // todo need mapper here
        return new GrpcTickerResponse
        {
            Id = item.Id,
            Name = item.Name,
            Symbol = item.Symbol,
            DecimalPoint = item.DecimalPoint,
            ExchangeName = item.Exchange.Name
        };
    }
}
using AutoMapper;
using Common.Grpc;
using Common.Security.Attributes;
using Common.Security.Enums;
using Grpc.Core;
using Market.Application.Features.GetPricesForPlugin.Request;
using Market.Application.Features.GetTickerPrices;
using MediatR;

namespace Market.API.Grpc;

public class GrpcPriceController(IMapper mapper, IMediator mediator) : GrpcPriceService.GrpcPriceServiceBase
{
    [HasPermission(Permissions.Enum.ViewMarketData)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcGetPricesResponse> GetPricesForPlugin(GrpcGetPricesRequest request,
        ServerCallContext context)
    {
        var input = mapper.Map<GetPricesForPluginQuery>(request);
        var data = await mediator.Send(input);
        var output = mapper.Map<GrpcGetPricesResponse>(data);
        return output;
    }

    [HasPermission(Permissions.Enum.ViewMarketData)]
    [HasRole(Roles.Enum.Admin)]
    public override async Task<GrpcGetPricesResponse> GetTickerPrices(GrpcGetTickerPricesRequest request,
        ServerCallContext context)
    {
        var input = mapper.Map<GetTickerPricesRequest>(request);
        var data = await mediator.Send(input);
        var output = mapper.Map<GrpcGetPricesResponse>(data);
        return output;
    }
}
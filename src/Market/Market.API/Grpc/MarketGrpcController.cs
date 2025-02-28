﻿using AutoMapper;
using Common.Grpc;
using Grpc.Core;
using Market.Application.Features.GetPricesForPlugin.Request;
using MediatR;

namespace Market.API.Grpc;

public class MarketGrpcController(IMapper mapper, IMediator mediator) : GrpcPriceController.GrpcPriceControllerBase
{
    public override async Task<GrpcGetPricesResponse> GetPricesForPlugin(GrpcGetPricesRequest request,
        ServerCallContext context)
    {
        var input = mapper.Map<GetPricesForPluginQuery>(request);
        var data = await mediator.Send(input);
        var output = mapper.Map<GrpcGetPricesResponse>(data);
        return output;
    }
}
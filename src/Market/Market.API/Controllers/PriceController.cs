using Common.Core.DTOs;
using Common.Grpc;
using Market.Application.Features.GetPricesForPlugin.Request;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class PriceController(IMediator mediator, ILogger<PriceController> logger) : ControllerBase
{
    private readonly GrpcPriceService.GrpcPriceServiceClient _client;

    // todo skip controller. !! implement grpc !!
    [HttpPost]
    public async Task<IEnumerable<PriceDto>> Get([FromBody] GetPricesForPluginQuery model)
    {
        // var query = new GetPricesForPluginQuery(model.TickerId, model.Timeframe, model.StartDate, model.EndDate);
        var r = await mediator.Send(model, CancellationToken.None);
        return r;
    }
}
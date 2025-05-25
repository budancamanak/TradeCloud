using AutoMapper;
using Backend.Application.Features.Chart.ExecutionPrices;
using Common.Application.Repositories;
using Common.Core.DTOs;
using Common.Logging.Events.Backend;
using Common.Security.Attributes;
using Common.Security.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ChartController(
    ILogger<AnalysisExecutionsController> logger,
    IHttpContextAccessor contextAccessor,
    ICacheService cache,
    IMediator mediator,
    IMapper mapper
)
{
    [HttpGet("Execution/{executionId:int}/Prices")]
    [HasPermission(Permissions.Enum.ViewMarketData, Permissions.Enum.ViewResults)]
    [HasRole(Roles.Enum.Admin, Roles.Enum.Analyst, Roles.Enum.QA)]
    public async Task<List<PriceDto>> GetExecutionPrices(int executionId)
    {
        logger.LogInformation(ChartLogEvents.ExecutionPrices, "Fetching analysis execution price information");
        var request = new GetExecutionPricesRequest
        {
            ExecutionId = executionId
        };
        var result = await mediator.Send(request);
        return result;
    }
}
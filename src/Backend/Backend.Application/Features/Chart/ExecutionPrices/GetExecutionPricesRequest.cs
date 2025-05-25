using Common.Core.DTOs;
using MediatR;

namespace Backend.Application.Features.Chart.ExecutionPrices;

public class GetExecutionPricesRequest : IRequest<List<PriceDto>>
{
    public int ExecutionId { get; set; }
}
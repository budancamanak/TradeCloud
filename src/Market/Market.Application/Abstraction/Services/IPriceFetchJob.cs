using Common.Core.DTOs;
using Common.Core.Models;
using Market.Application.Models;

namespace Market.Application.Abstraction.Services;

public interface IPriceFetchJob
{
    public List<PriceDto> FetchedPrices { get; set; }
    public PriceFetchRequest? ActiveRequest { get; set; }

    /// <summary>
    /// Will connect to exchange and fetch price information for pages.
    /// Upon successful completion will call <see cref="OnFinish"/> method
    /// On any exception or unwanted behaviours, will call <see cref="OnError"/> method
    /// </summary>
    /// <param name="pages"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MethodResponse> StartFetchPrices(IList<PriceFetchPages> pages, PriceFetchRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Will be called by fetch prices when fetch operation finished successfully.
    /// Will send an event via <see cref="MediatR"/>
    /// todo Might replace <see cref="MediatR"/> with Kafka/RabbitMQ in the future if price fetch operation can be done on separate microservice
    /// </summary>
    /// <returns></returns>
    Task<MethodResponse> OnFinish();

    /// <summary>
    /// Will be called by fetch prices when fetch operation fails to finish.
    /// Will send an event via <see cref="MediatR"/>
    /// todo Might replace <see cref="MediatR"/> with Kafka/RabbitMQ in the future if price fetch operation can be done on separate microservice
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    Task<MethodResponse> OnError(Exception exception);
}
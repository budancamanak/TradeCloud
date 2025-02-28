using Common.Core.DTOs;

namespace Market.Application.Abstraction.Services;

/// <summary>
/// 5400 6170 2581 2617
/// 06/25
/// 630
/// </summary>
public interface IPriceFetchCalculatorService
{
    /// <summary>
    /// will check if supplied price list complies with start & end dates
    /// will return true if we need prices to fetch
    /// will return false if prices we have is ok with start & end dates
    /// </summary>
    /// <param name="priceInfo"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns>will return true if we need prices to fetch otherwise false</returns>
    bool CheckPriceFetchIfNeeded(IList<PriceDto>? priceInfo, DateTime start, DateTime end);
}
using Common.Core.DTOs;
using Market.Application.Abstraction.Services;

namespace Market.Infrastructure.Services;

public class PriceFetchCalculatorService : IPriceFetchCalculatorService
{
    public bool CheckPriceFetchIfNeeded(IList<PriceDto>? priceInfo, DateTime start, DateTime end)
    {
        if (ListNullOrEmpty(priceInfo)) return true;
        if (start == default) return true;
        if (end == default) return true;
        if (priceInfo![0].Timestamp <= start && priceInfo!.Last().Timestamp >= end)
        {
            return false;
        }

        return true;
    }

    private static bool ListNullOrEmpty<T>(IList<T>? list)
    {
        return list == null || list.Count == 0;
    }
}
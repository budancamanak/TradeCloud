using Common.Application.Repositories;
using Common.Core.Enums;
using Common.Core.Models;
using Market.Domain.Entities;

namespace Market.Application.Abstraction.Repositories;

public interface IPriceRepository : IAsyncRepository<Price>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tickerId"></param>
    /// <param name="timeframe"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Throws when tickerId is negative or zero</exception>
    /// <exception cref="ArgumentNullException">Throws when startDate has default value</exception>
    Task<List<Price>> GetTickerPricesAsync(int tickerId, Timeframe timeframe, DateTime start, DateTime? end);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="priceInfo"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Throws when price list is null</exception>
    /// <exception cref="ArgumentOutOfRangeException">Throws when price list is empty</exception>
    /// <exception cref="ArgumentException">Throws when all prices in list fail to validate</exception>
    Task<MethodResponse> SavePricesAsync(IList<Price> priceInfo);
    Task<Price> GetPriceByTimestamp(int tickerId, DateTime timestamp);
    Task<MethodResponse> DeleteTickerPricesAsync(int tickerId, Timeframe timeframe, DateTime start, DateTime? end);
}
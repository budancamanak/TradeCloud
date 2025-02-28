using Common.Application.Repositories;
using Common.Core.Models;
using Market.Domain.Entities;

namespace Market.Application.Abstraction.Repositories;

public interface ITickerRepository : IAsyncRepository<Ticker>
{
    Task<IEnumerable<Ticker>> GetAllByExchangeAsync(int exchangeId);
    Task<Ticker> GetBySymbolAsync(string symbol);
}
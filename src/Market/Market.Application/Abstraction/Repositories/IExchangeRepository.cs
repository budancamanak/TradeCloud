using Common.Application.Repositories;
using Market.Domain.Entities;

namespace Market.Application.Abstraction.Repositories;

public interface IExchangeRepository : IAsyncRepository<Exchange>
{
}
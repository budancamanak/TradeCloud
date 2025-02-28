using Common.Core.DTOs;

namespace Backend.Application.Abstraction.Services;

public interface ITickerService
{
    Task<List<TickerDto>> GetAvailableTickers();
    Task<TickerDto> GetTickerWithId(int id);
    Task<TickerDto> GetTickerWithSymbol(string symbol);
}
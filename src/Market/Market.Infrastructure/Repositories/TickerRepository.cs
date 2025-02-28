using Ardalis.GuardClauses;
using Common.Core.Extensions;
using Common.Core.Models;
using Common.Web.Exceptions;
using FluentValidation;
using Market.Application.Abstraction.Repositories;
using Market.Application.Exceptions;
using Market.Domain.Entities;
using Market.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class TickerRepository(MarketDbContext dbContext, IValidator<Ticker> validator) : ITickerRepository
{
    public async Task<Ticker> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var ticker = await dbContext.Tickers.Include(f => f.Exchange).FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.Null(ticker);
        return ticker;
    }

    public async Task<IEnumerable<Ticker>> GetAllByExchangeAsync(int exchangeId)
    {
        Guard.Against.NegativeOrZero(exchangeId);
        var exchange = await dbContext.Exchanges.FirstOrDefaultAsync(f => f.Id == exchangeId);
        Guard.Against.Null(exchange);
        var items = await dbContext.Tickers.Where(f => f.ExchangeId == exchangeId).ToListAsync();
        return items;
    }

    public async Task<Ticker> GetBySymbolAsync(string symbol)
    {
        Guard.Against.NullOrEmpty(symbol);
        var ticker = await dbContext.Tickers.Include(f => f.Exchange).FirstOrDefaultAsync(f => f.Symbol == symbol);
        Guard.Against.Null(ticker, message: "Couldn't find ticker",
            exceptionCreator: () => new RequestValidationException("Couldn't find ticker"));
        return ticker;
    }

    public async Task<List<Ticker>> GetAllAsync()
    {
        var items = await dbContext.Tickers.Include(f => f.Exchange).ToListAsync();
        return items;
    }

    public async Task<MethodResponse> AddAsync(Ticker ticker)
    {
        Guard.Against.Null(ticker);
        await validator.ValidateAndThrowAsync(ticker);

        var existing = await
            dbContext.Tickers.FirstOrDefaultAsync(f => f.Symbol == ticker.Symbol && f.ExchangeId == ticker.ExchangeId);
        Guard.Against.NonNull(existing, "Ticker already saved",
            () => new AlreadySavedException("Ticker already saved"));
        dbContext.Tickers.Add(ticker);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save ticker");
        return MethodResponse.Success(ticker.Id, "Ticker saved");
    }

    public async Task<MethodResponse> UpdateAsync(Ticker ticker)
    {
        Guard.Against.Null(ticker);
        Guard.Against.NegativeOrZero(ticker.Id);
        var existing = await dbContext.Tickers.FirstOrDefaultAsync(f => f.Id == ticker.Id);
        Guard.Against.Null(existing, "Ticker not found");
        Guard.Against.NotFound(ticker.Id, existing);

        existing.ExchangeId = ticker.ExchangeId;
        existing.Name = ticker.Name;
        existing.Symbol = ticker.Symbol;
        existing.DecimalPoint = ticker.DecimalPoint;
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to update ticker");
        return MethodResponse.Success(ticker.Id, "Ticker updated");
    }

    public async Task<MethodResponse> DeleteAsync(Ticker ticker)
    {
        Guard.Against.Null(ticker, message: "Ticker is null");
        var existing =
            await dbContext.Tickers.FirstOrDefaultAsync(f => f.Id == ticker.Id);
        Guard.Against.NotFound(ticker.Id, existing);

        return await _DeleteTicker(existing);
    }

    public async Task<MethodResponse> DeleteAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = await dbContext.Tickers.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.NotFound(id, existing);
        return await _DeleteTicker(existing);
    }


    private async Task<MethodResponse> _DeleteTicker(Ticker existing)
    {
        dbContext.Tickers.Remove(existing);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to delete ticker");
        return MethodResponse.Success(existing.Id, "Ticker deleted");
    }
}
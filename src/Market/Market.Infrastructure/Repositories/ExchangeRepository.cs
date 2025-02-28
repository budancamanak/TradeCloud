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

public class ExchangeRepository(MarketDbContext dbContext, IValidator<Exchange> validator) : IExchangeRepository
{
    public async Task<Exchange> GetByIdAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var exchange = await dbContext.Exchanges.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.Null(exchange);
        return exchange;
    }

    public async Task<List<Exchange>> GetAllAsync()
    {
        var items = await dbContext.Exchanges.ToListAsync();
        return items;
    }

    public async Task<MethodResponse> AddAsync(Exchange exchange)
    {
        Guard.Against.Null(exchange);
        await validator.ValidateAndThrowAsync(exchange);

        var existing = await dbContext.Exchanges.FirstOrDefaultAsync(f => f.Name == exchange.Name);
        Guard.Against.NonNull(existing, "Exchange already exists",
            () => new AlreadySavedException("Exchange already exists"));
        dbContext.Exchanges.Add(exchange);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to save exchange");
        return MethodResponse.Success(exchange.Id, "Exchange saved");
    }

    public async Task<MethodResponse> UpdateAsync(Exchange exchange)
    {
        Guard.Against.Null(exchange);
        Guard.Against.NegativeOrZero(exchange.Id);
        var existing = await dbContext.Exchanges.FirstOrDefaultAsync(f => f.Id == exchange.Id);
        Guard.Against.NotFound(exchange.Id, existing);


        existing.Name = exchange.Name;
        existing.ConnectionUrl = exchange.ConnectionUrl;
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to update exchange");
        return MethodResponse.Success(exchange.Id, "Exchange updated");
    }

    public async Task<MethodResponse> DeleteAsync(Exchange exchange)
    {
        Guard.Against.Null(exchange);
        Guard.Against.NegativeOrZero(exchange.Id);
        var existing = await dbContext.Exchanges.FirstOrDefaultAsync(f => f.Id == exchange.Id);
        Guard.Against.NotFound(exchange.Id, existing);
        if (exchange.Name != existing.Name) throw new ArgumentException("Exchange names mismatch");
        return await _DeleteExchange(existing);
    }

    public async Task<MethodResponse> DeleteAsync(int id)
    {
        Guard.Against.NegativeOrZero(id);
        var existing = await dbContext.Exchanges.FirstOrDefaultAsync(f => f.Id == id);
        Guard.Against.NotFound(id, existing);
        return await _DeleteExchange(existing);
    }

    private async Task<MethodResponse> _DeleteExchange(Exchange existing)
    {
        dbContext.Exchanges.Remove(existing);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to delete exchange");
        return MethodResponse.Success(existing.Id, "Exchange deleted");
    }
}
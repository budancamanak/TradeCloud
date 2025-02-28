using Ardalis.GuardClauses;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Models;
using FluentValidation;
using Market.Application.Abstraction.Repositories;
using Market.Domain.Entities;
using Market.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Repositories;

public class PriceRepository(MarketDbContext dbContext, IValidator<Price> validator)
    : IPriceRepository
{
    public Task<Price> GetByIdAsync(int tickerId)
    {
        throw new NotImplementedException("Not supported");
    }


    public async Task<Price> GetPriceByTimestamp(int tickerId, DateTime timestamp)
    {
        Guard.Against.Null(timestamp);
        Guard.Against.NegativeOrZero(tickerId);
        var price = await dbContext.Prices.FirstOrDefaultAsync(f => f.TickerId == tickerId && f.Timestamp == timestamp);
        Guard.Against.Null(price);
        return price;
    }

    // todo probably will need to add pagination to here
    public async Task<List<Price>> GetTickerPricesAsync(int tickerId, Timeframe timeframe, DateTime start,
        DateTime? end)
    {
        Guard.Against.NegativeOrZero(tickerId);
        Guard.Against.NullDate(start);
        end ??= DateTime.UtcNow;

        var items = await dbContext.Prices
            .Where(f => f.TickerId == tickerId && f.Timeframe == timeframe && f.Timestamp >= start &&
                        f.Timestamp <= end)
            .OrderBy(f => f.Timestamp)
            .ToListAsync();
        return items;
    }

    public Task<List<Price>> GetAllAsync()
    {
        // var items = await dbContext.Prices.ToListAsync();
        // return items;
        throw new NotImplementedException("Not supported");
    }

    public Task<MethodResponse> AddAsync(Price price)
    {
        throw new NotImplementedException("Not supported");
        // Guard.Against.Null(price);
        // await validator.ValidateAndThrowAsync(price);
        // var existing = await
        //     dbContext.Prices.FirstOrDefaultAsync(f => f.Timestamp == price.Timestamp && f.TickerId == price.TickerId);
        // Guard.Against.NonNull(existing, "Price already saved", () => new AlreadySavedException());
        // dbContext.Prices.Add(price);
        // var result = await dbContext.SaveChangesAsync();
        // if (result == 0) return MethodResponse.Error("Failed to save price");
        // return MethodResponse.Success(result, "Price saved");
    }

    public async Task<MethodResponse> SavePricesAsync(IList<Price> priceInfo)
    {
        Guard.Against.NullOrZeroLengthArray(priceInfo);
        var existingCount = 0;
        var addCount = 0;
        var invalidCount = 0;
        foreach (var price in priceInfo)
        {
            var valResult = await validator.ValidateAsync(price);
            if (!valResult.IsValid)
            {
                invalidCount++;
                continue;
            }

            var existing = await
                dbContext.Prices.FirstOrDefaultAsync(
                    f => f.Timestamp == price.Timestamp && f.TickerId == price.TickerId &&
                         f.Timeframe == price.Timeframe);
            if (existing != null)
            {
                existingCount++;
                continue;
            }

            dbContext.Prices.Add(price);
            addCount++;
        }

        Guard.Against.Expression(f => f == priceInfo.Count, invalidCount, "All prices are invalid");
        if (existingCount == priceInfo.Count)
            return MethodResponse.Success(existingCount, "Prices are already saved.");
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error(result, "Failed to save prices");
        return MethodResponse.Success(result, "Prices saved");
    }

    public async Task<MethodResponse> UpdateAsync(Price price)
    {
        Guard.Against.Null(price);
        await validator.ValidateAndThrowAsync(price);
        var existing = await
            dbContext.Prices.FirstOrDefaultAsync(f => f.Timestamp == price.Timestamp && f.TickerId == price.TickerId);
        Guard.Against.NotFound(price.Timestamp, existing);
        existing.Close = price.Close;
        existing.High = price.High;
        existing.Low = price.Low;
        existing.Open = price.Open;

        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to update price");
        return MethodResponse.Success(result, "Price updated");
    }

    public async Task<MethodResponse> DeleteTickerPricesAsync(int tickerId, Timeframe timeframe, DateTime start,
        DateTime? end)
    {
        var itemsToDelete = await GetTickerPricesAsync(tickerId, timeframe, start, end);
        dbContext.Prices.RemoveRange(itemsToDelete);
        var result = await dbContext.SaveChangesAsync();
        if (result == 0) return MethodResponse.Error("Failed to remove prices");
        return MethodResponse.Success(result, "Prices removed");
    }

    public async Task<MethodResponse> DeleteAsync(Price price)
    {
        // Guard.Against.Null(price, message: "Price is null");
        // var existing = await
        //     dbContext.Prices.FirstOrDefaultAsync(f => f.Timestamp == price.Timestamp && f.TickerId == price.TickerId);
        // Guard.Against.Null(existing, message: "Price not found");
        //
        // return await _DeletePrice(existing);
        throw new NotImplementedException("not supported");
    }

    public Task<MethodResponse> DeleteAsync(int id)
    {
        throw new NotImplementedException("not supported");
    }

    // private async Task<MethodResponse> _DeletePrice(Price existing)
    // {
    //     dbContext.Prices.Remove(existing);
    //     var result = await dbContext.SaveChangesAsync();
    //     if (result == 0) return MethodResponse.Error("Failed to delete price");
    //     return MethodResponse.Success(result, "Price deleted");
    // }
}
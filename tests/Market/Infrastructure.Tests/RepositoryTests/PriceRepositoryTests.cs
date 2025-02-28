using Common.Core.Enums;
using FluentAssertions;
using FluentValidation;
using Tests.Common.Data;
using Market.Application.Validators;
using Market.Domain.Entities;
using Market.Infrastructure.Data;
using Market.Infrastructure.Repositories;
using Tests.Common;

namespace Infrastructure.Tests.RepositoryTests;

[TestFixture]
public class PriceRepositoryTests() :
    BaseRepositoryTest<MarketDbContext, MarketDatabaseFixture, PriceRepository, PriceValidator>(
        "MarketDbPriceRepositoryTests")
{
    /*
     *      Most Important Ones
     *  GetByTicker
     *  SaveMultiplePrices
     *  GetByPriceAndDateRange
     *
     */

    #region ListPrices Tests

    [Test]
    [Order(0)]
    public async Task ListPriceSingle_GetById_ShouldFail()
    {
        var id = 1;
        // Act
        Func<Task> getAction = async () => { await Repository.GetByIdAsync(id); };

        // Assert
        await getAction.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    [Order(1)]
    public async Task ListPrices_GetByAll_ShouldFail()
    {
        Func<Task> getAction = async () => { await Repository.GetAllAsync(); };

        // Assert
        await getAction.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    [Order(2)]
    public async Task ListPriceSingle_GetByTimestamp()
    {
        var priceToCheck = MarketServiceTestData.Instance.Prices[0];

        // Act
        var result = await Repository.GetPriceByTimestamp(priceToCheck.TickerId, priceToCheck.Timestamp);
        result.Should().NotBeNull();
        result.Low.Should().Be(priceToCheck.Low);
        result.High.Should().Be(priceToCheck.High);
        result.Close.Should().Be(priceToCheck.Close);
        result.Open.Should().Be(priceToCheck.Open);
    }

    [Test]
    [Order(3)]
    public async Task ListPriceSingle_GetByTimestamp_NegativeTickerId_ShouldFail()
    {
        var _now = DateTime.UtcNow;
        var tickerId = -1;
        // Act
        Func<Task> getAction = async () => { await Repository.GetPriceByTimestamp(tickerId, _now); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(4)]
    public async Task ListPriceSingle_GetByTimestamp_NotExisting_ShouldFail()
    {
        var _now = DateTime.UtcNow;
        // Act
        Func<Task> getAction = async () => { await Repository.GetPriceByTimestamp(1, _now); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(5)]
    public async Task ListPriceSingle_GetByTimestamp_NullTimestamp_ShouldFail()
    {
        DateTime _now = default;
        // Act
        Func<Task> getAction = async () => { await Repository.GetPriceByTimestamp(1, _now); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(6)]
    public async Task ListPrices_GetByTickerAndDateRange()
    {
        var endDate = MarketServiceTestData.Now;
        var start = endDate.AddHours(-2);
        var tickerId = 1;
        var tf = Timeframe.Hour1;

        var result = (await Repository.GetTickerPricesAsync(tickerId, tf, start, endDate)).ToList();

        result.Should().NotBeNullOrEmpty();
        var output = MarketServiceTestData.Instance.Prices
            .Where(f => f.TickerId == tickerId && f.Timestamp >= start && f.Timestamp <= endDate)
            .ToList();
        result.Count.Should().Be(output.Count);
    }

    [Test]
    [Order(7)]
    public async Task ListPrices_GetByTickerAndDateStartNoEndDate()
    {
        var start = MarketServiceTestData.Now.AddHours(-2);
        var tickerId = 1;

        var result = (await Repository.GetTickerPricesAsync(tickerId, Timeframe.Hour1, start, null)).ToList();

        result.Should().NotBeNullOrEmpty();
        var output = MarketServiceTestData.Instance.Prices
            .Where(f => f.TickerId == tickerId && f.Timestamp >= start && f.Timestamp <= MarketServiceTestData.Now)
            .ToList();
        result.Count.Should().Be(output.Count);
    }

    [Test]
    [Order(8)]
    public async Task ListPrices_GetByTickerAndDate_NotExistingDate()
    {
        var start = MarketServiceTestData.Now.AddDays(-20);
        var end = MarketServiceTestData.Now.AddDays(-19);
        var tickerId = 1;

        var result = await Repository.GetTickerPricesAsync(tickerId, Timeframe.Hour1, start, end);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Test]
    [Order(9)]
    public async Task ListPrices_GetByTickerAndDateRange_NegativeTicker_ShouldFail()
    {
        var endDate = MarketServiceTestData.Now;
        var start = endDate.AddHours(-2);
        var tickerId = -1;

        Func<Task> getAction = async () =>
        {
            await Repository.GetTickerPricesAsync(tickerId, Timeframe.Hour1, start, endDate);
        };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(10)]
    public async Task ListPrices_GetByTickerAndDateRange_NullStartDate_ShouldFail()
    {
        var endDate = MarketServiceTestData.Now;

        var tickerId = 1;

        Func<Task> getAction = async () =>
        {
            await Repository.GetTickerPricesAsync(tickerId, Timeframe.Hour1, default, endDate);
        };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region AddPrices Tests

    [Test]
    [Order(11)]
    public async Task AddSinglePrice_SaveToDatabase_ShouldFail()
    {
        // Arrange
        var price = new Price { Timestamp = DateTime.UtcNow, Close = 1, High = 1, Low = 1, Open = 1, TickerId = 1 };
        Func<Task> getAction = async () => { await Repository.AddAsync(price); };

        // Assert
        await getAction.Should().ThrowAsync<NotImplementedException>();
    }

    [Test]
    [Order(12)]
    public async Task AddPrices_SaveToDatabase()
    {
        var prices = new Price[]
        {
            new Price
            {
                Close = 1, High = 1, Low = 1, Open = 1, TickerId = 1,
                Timeframe = Timeframe.Hour1, Timestamp = MarketServiceTestData.Now.AddHours(-15)
            }
        };
        // Act
        var result = await Repository.SavePricesAsync(prices.ToList());

        // Assert
        result.IsSuccess.Should().Be(true);
        var price = prices[0];
        var savedPrice = await Repository.GetPriceByTimestamp(price.TickerId, price.Timestamp);
        savedPrice.Should().NotBeNull();
        savedPrice.High.Should().Be(price.High);
        savedPrice.Low.Should().Be(price.Low);
    }

    [Test]
    [Order(13)]
    public async Task AddPrices_SaveToDatabase_EmptyPrices_ShouldFail()
    {
        var prices = new Price[]
        {
        };
        Func<Task> getAction = async () => { await Repository.SavePricesAsync(prices.ToList()); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Test]
    [Order(14)]
    public async Task AddPrices_SaveToDatabase_NullPriceList_ShouldFail()
    {
        Func<Task> getAction = async () => { await Repository.SavePricesAsync(null); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(15)]
    public async Task AddPrices_SaveToDatabase_AllWrongPrices_ShouldFail()
    {
        var prices = new Price[]
        {
            new Price { Close = 1, High = 1, Low = 1, Open = 1, TickerId = -1, Timestamp = MarketServiceTestData.Now }
        };
        Func<Task> getAction = async () => { await Repository.SavePricesAsync(prices.ToList()); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(16)]
    public async Task AddPrices_SaveToDatabase_SomeWrongPrices()
    {
        var prices = new Price[]
        {
            new Price
            {
                Timeframe = Timeframe.Day1, Close = 1, High = 1, Low = 1, Open = 1, TickerId = 1,
                Timestamp = MarketServiceTestData.Now.AddDays(-1)
            },
            new Price
            {
                Timeframe = Timeframe.Day1, Close = 1, High = 1, Low = 1, Open = 1, TickerId = -1,
                Timestamp = MarketServiceTestData.Now.AddDays(-2)
            }
        };
        var result = await Repository.SavePricesAsync(prices.ToList());
        result.IsSuccess.Should().BeTrue();
        result.Id.Should().Be(1);
    }

    #endregion

    #region UpdatePrices Tests

    [Test]
    [Order(17)]
    public async Task UpdatePrices_UpdateSingleToDatabase()
    {
        var price = MarketServiceTestData.Instance.Prices[0];
        price.Close = 10;
        var result = await Repository.UpdateAsync(price);
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    [Order(18)]
    public async Task UpdatePrices_UpdateNullPrice_ShouldFail()
    {
        Func<Task> getAction = async () => { await Repository.UpdateAsync(null); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(19)]
    public async Task UpdatePrices_UpdateNullPriceTimestamp_ShouldFail()
    {
        var price = new Price { TickerId = 1, Timestamp = default };
        Func<Task> getAction = async () => { await Repository.UpdateAsync(price); };

        // Assert
        await getAction.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [Order(20)]
    public async Task UpdatePrices_UpdateNotExisting_ShouldFail()
    {
        var price = new Price { TickerId = 11111, Timestamp = MarketServiceTestData.Now };
        Func<Task> getAction = async () => { await Repository.UpdateAsync(price); };

        // Assert
        await getAction.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [Order(21)]
    public async Task UpdatePrices_UpdateWrongValues_ShouldFail()
    {
        var price = new Price { TickerId = -1, Timestamp = MarketServiceTestData.Now };
        Func<Task> getAction = async () => { await Repository.UpdateAsync(price); };

        // Assert
        await getAction.Should().ThrowAsync<ValidationException>();
    }

    #endregion

    #region DeletePrices Tests

    [Test]
    [Order(22)]
    public async Task DeletePrices_DeletePricesFromDatabase()
    {
        var startDate = MarketServiceTestData.Now.AddHours(-2);
        var itemsToRemove = MarketServiceTestData.Instance.Prices
            .Where(f => f.TickerId == 1 && f.Timestamp >= startDate).ToList();
        var result = await Repository.DeleteTickerPricesAsync(1, Timeframe.Hour1, startDate, default);
        result.IsSuccess.Should().BeTrue();
        result.Id.Should().Be(itemsToRemove.Count);
    }

    [Test]
    [Order(23)]
    public async Task DeletePrices_DeleteVeryBigTicker_ShouldFail()
    {
        var tid = Int32.MaxValue;
        tid += 10;
        Func<Task> getAction = async () =>
        {
            await Repository.DeleteTickerPricesAsync(tid, Timeframe.Hour1, MarketServiceTestData.Now.AddHours(-5),
                default);
        };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(24)]
    public async Task DeletePrices_DeleteNegativeTicker_ShouldFail()
    {
        Func<Task> getAction = async () =>
        {
            await Repository.DeleteTickerPricesAsync(-1, Timeframe.Hour1, MarketServiceTestData.Now.AddHours(-5),
                default);
        };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(25)]
    public async Task DeletePrices_DeleteNullStartDate_ShouldFail()
    {
        Func<Task> getAction = async () =>
        {
            await Repository.DeleteTickerPricesAsync(1, Timeframe.Hour1, default, default);
        };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion
}
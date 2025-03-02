using Ardalis.GuardClauses;
using Common.Web.Exceptions;
using FluentAssertions;
using FluentValidation;
using Tests.Common.Data;
using Market.Application.Exceptions;
using Market.Application.Validators;
using Market.Domain.Entities;
using Market.Infrastructure.Data;
using Market.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Tests.Common;


namespace Infrastructure.Tests.RepositoryTests;

[TestFixture]
public class TickerRepositoryTests() :
    BaseRepositoryTest<MarketDbContext, MarketDatabaseFixture, TickerRepository, TickerValidator>(
        "MarketDbTickerRepositoryTest")
{
    #region ListTicker Tests

    [Test]
    [Order(0)]
    public async Task ListTickers_GetById()
    {
        // Arrange
        var ticker = 3;

        // Act
        var result = await Repository.GetByIdAsync(ticker);
        result.Should().NotBeNull();
        result.Name.Should().Be("Ripple");
        result.Symbol.Should().Be("XRP/USDT");
    }

    [Test]
    [Order(15)]
    public async Task ListTickers_GetById_NotExisting_ShouldFail()
    {
        // Arrange
        var ticker = 1;

        // Act
        Func<Task> getAction = async () => { await Repository.GetByIdAsync(ticker); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(0)]
    public async Task ListTickers_GetByAll()
    {
        // Arrange
        var tickerSize = MarketServiceTestData.Instance.Tickers.Count;

        // Act
        var items = await Repository.GetAllAsync();

        // Assert
        items.Should().NotBeNull();
        items.Count().Should().Be(tickerSize);
        items.ElementAt(0).Should().NotBeNull();
        items.ElementAt(0).Name.Should().Be(MarketServiceTestData.Instance.Tickers[0].Name);
    }

    [Test]
    [Order(0)]
    public async Task ListTickers_GetByAllExchange()
    {
        // Arrange
        int exchangeId = 1;
        var tData = MarketServiceTestData.Instance.Tickers.Where(f => f.ExchangeId == exchangeId);
        var tickerSize = tData.Count();

        // Act
        var items = await Repository.GetAllByExchangeAsync(exchangeId);

        // Assert
        items.Should().NotBeNull();
        items.Count().Should().Be(tickerSize);
        items.ElementAt(0).Should().NotBeNull();
        items.ElementAt(0).Name.Should().Be(MarketServiceTestData.Instance.Tickers[0].Name);
    }

    [Test]
    [Order(0)]
    public async Task ListTickers_GetByAllExchange_NegativeExchangeId_ShouldFail()
    {
        // Arrange
        int exchangeId = -1;

        // Act
        Func<Task> getAction = async () => { await Repository.GetAllByExchangeAsync(exchangeId); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(0)]
    public async Task ListTickers_GetByAllExchange_NonExistingExchange_ShouldFail()
    {
        // Arrange
        int exchangeId = 111;

        // Act
        Func<Task> getAction = async () => { await Repository.GetAllByExchangeAsync(exchangeId); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region AddTicker Tests

    [Test]
    [Order(1)]
    public async Task AddTicker_SaveToDatabase()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL", ExchangeId = 1, Name = "Apple", DecimalPoint = 1 };

        // Act
        var result = await Repository.AddAsync(ticker);
        result.IsSuccess.Should().Be(true);
        result.Id.Should().BeGreaterThan(0);

        // Assert
        var savedTicker = await DbContext.Tickers.FindAsync(result.Id);
        savedTicker.Should().NotBeNull();
        savedTicker.Symbol.Should().Be("AAPL");
    }

    [Test]
    [Order(2)]
    public async Task AddTicker_SaveDuplicate_ShouldFail()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL", ExchangeId = 1, Name = "Apple", DecimalPoint = 1 };

        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(ticker); };

        // Assert
        await saveAction.Should().ThrowAsync<AlreadySavedException>();
    }

    [Test]
    [Order(3)]
    public async Task AddTicker_SaveMissingValues_ShouldFail()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "APPL", ExchangeId = 2 };

        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(ticker); };
        await saveAction.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    [Order(4)]
    public async Task AddTicker_SaveNull_ShouldFail()
    {
        // Arrange
        Ticker ticker = null;

        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(ticker); };
        await saveAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(5)]
    public async Task AddTicker_SaveNegativeExchangeId_ShouldFail()
    {
        // Arrange
        Ticker ticker = new Ticker { Symbol = "AAPL", ExchangeId = -1 };

        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(ticker); };
        await saveAction.Should().ThrowAsync<ValidationException>();
    }

    #endregion

    #region UpdateTicker Tests

    [Test]
    [Order(6)]
    public async Task UpdateTicker_UpdateToDatabase()
    {
        var ticker = new Ticker { Symbol = "AAPL2", ExchangeId = 1, Name = "Apple2", Id = 1 };

        // Act
        var result = await Repository.UpdateAsync(ticker);
        result.IsSuccess.Should().Be(true);

        // Assert
        var savedTicker = await Repository.GetByIdAsync(ticker.Id);
        savedTicker.Should().NotBeNull();
        savedTicker.Symbol.Should().Be("AAPL2");
    }

    [Test]
    public async Task UpdateTicker_UpdateNegativeId_ShouldFail()
    {
        var ticker = new Ticker { Symbol = "AAPL2", ExchangeId = 1, Name = "Apple2", Id = -1 };

        // Act
        Func<Task> saveAction = async () => { await Repository.UpdateAsync(ticker); };

        // Assert
        await saveAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(7)]
    public async Task UpdateTicker_UpdateNotFound_ShouldFail()
    {
        // Arrange
        var ticker = new Ticker { Symbol = "AAPL", ExchangeId = 1, Name = "Apple", Id = 111, DecimalPoint = 1 };

        // Act
        Func<Task> saveAction = async () => { await Repository.UpdateAsync(ticker); };

        // Assert
        await saveAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(8)]
    public async Task UpdateTicker_UpdateNull_ShouldFail()
    {
        // Arrange
        Ticker ticker = null;

        // Act
        Func<Task> saveAction = async () => { await Repository.UpdateAsync(ticker); };
        await saveAction.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    #region DeleteTicker Tests

    [Test]
    [Order(9)]
    public async Task DeleteTicker_ByTicker()
    {
        var ticker = new Ticker { Symbol = "AAPL2", ExchangeId = 1, Name = "Apple2", Id = 1 };

        // Act
        var result = await Repository.DeleteAsync(ticker);
        result.IsSuccess.Should().Be(true);

        // Assert
        var savedTicker = await DbContext.Tickers.FindAsync(ticker.Id);
        savedTicker.Should().BeNull();
    }

    [Test]
    [Order(10)]
    public async Task DeleteTicker_ByTickerId()
    {
        var tickerId = 3;

        // Act
        var result = await Repository.DeleteAsync(tickerId);
        result.IsSuccess.Should().Be(true);

        // Assert
        var savedTicker = await DbContext.Tickers.FindAsync(tickerId);
        savedTicker.Should().BeNull();
    }

    [Test]
    [Order(11)]
    public async Task DeleteTicker_DeleteNotExisting_ByTickerId_ShouldFail()
    {
        var tickerId = 1;

        // Act
        Func<Task> saveAction = async () => { await Repository.DeleteAsync(tickerId); };

        // Assert
        await saveAction.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    [Order(12)]
    public async Task DeleteTicker_DeleteNotExisting_ByTicker_ShouldFail()
    {
        var ticker = new Ticker { Symbol = "AAPL2", ExchangeId = 1, Name = "Apple2", Id = 1 };

        // Act
        Func<Task> saveAction = async () => { await Repository.DeleteAsync(ticker); };

        // Assert
        await saveAction.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    [Order(13)]
    public async Task DeleteTicker_DeleteNegativeId_ShouldFail()
    {
        var tickerId = -1;

        // Act
        Func<Task> saveAction = async () => { await Repository.DeleteAsync(tickerId); };

        // Assert
        await saveAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    [Order(13)]
    public async Task DeleteTicker_DeleteNull_ShouldFail()
    {
        Ticker tickerId = null;

        // Act
        Func<Task> saveAction = async () => { await Repository.DeleteAsync(tickerId); };

        // Assert
        await saveAction.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion
}
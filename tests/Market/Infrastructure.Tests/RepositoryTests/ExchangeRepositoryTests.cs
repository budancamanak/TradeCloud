using System.Text;
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
public class ExchangeRepositoryTests() :
    BaseRepositoryTest<MarketDbContext, MarketDatabaseFixture, ExchangeRepository, ExchangeValidator>(
        "MarketDbExchangeRepositoryTests")
{
    #region ListExchange Tests

    [Test]
    [Order(0)]
    public async Task ListExchanges_GetById()
    {
        // Arrange
        var exc = 1;

        // Act
        var result = await Repository.GetByIdAsync(exc);
        result.Should().NotBeNull();
        result.Name.Should().Be("Binance");
    }

    [Test]
    public async Task ListExchanges_GetById_NotExisting_ShouldFail()
    {
        var exc = 1111;
        Func<Task> getAction = async () => { await Repository.GetByIdAsync(exc); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    [Order(0)]
    public async Task ListExchanges_GetByAll()
    {
        // Arrange
        var excSize = MarketServiceTestData.Instance.Exchanges.Count;

        // Act
        var items = await Repository.GetAllAsync();

        // Assert
        items.Should().NotBeNull();
        items.Count().Should().Be(excSize);
        items.ElementAt(0).Should().NotBeNull();
        items.ElementAt(0).Name.Should().Be(MarketServiceTestData.Instance.Exchanges[0].Name);
    }

    [Test]
    [Order(0)]
    public async Task ListExchanges_GetById_NegativeExchangeId_ShouldFail()
    {
        // Arrange
        int exchangeId = -1;

        // Act
        Func<Task> getAction = async () => { await Repository.GetByIdAsync(exchangeId); };

        // Assert
        await getAction.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region AddExchange Tests

    [Test]
    public async Task AddExchange_SaveToDatabase()
    {
        // Arrange
        var exchange = new Exchange { Name = "TestExchange" };

        // Act
        var result = await Repository.AddAsync(exchange);
        result.IsSuccess.Should().Be(true);
        result.Id.Should().BeGreaterThan(0);

        // Assert
        var savedExchange = await DbContext.Exchanges.FindAsync(result.Id);
        savedExchange.Should().NotBeNull();
        savedExchange.Name.Should().Be(exchange.Name);
    }

    [Test]
    public async Task AddExchange_SaveDuplicate_ShouldFail()
    {
        // Arrange
        var exchange = new Exchange { Name = "Binance" };
        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(exchange); };

        // Assert
        await saveAction.Should().ThrowAsync<AlreadySavedException>();
    }

    [Test]
    public async Task AddExchange_SaveMissingValues_ShouldFail()
    {
        var exchange = new Exchange { };
        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(exchange); };
        await saveAction.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task AddExchange_SaveNull_ShouldFail()
    {
        // Arrange
        Exchange exchange = null;

        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(exchange); };
        await saveAction.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task AddExchange_SaveLongName_ShouldFail()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < 100; i++)
        {
            sb.Append('T');
        }

        var exchange = new Exchange { Name = sb.ToString() };
        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(exchange); };
        await saveAction.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task AddExchange_SaveShortName_ShouldFail()
    {
        var exchange = new Exchange { Name = "T" };
        // Act
        Func<Task> saveAction = async () => { await Repository.AddAsync(exchange); };
        await saveAction.Should().ThrowAsync<ValidationException>();
    }

    #endregion

    #region UpdateExchange Tests

    [Test]
    public async Task UpdateExchange_UpdateToDatabase()
    {
        var exchange = new Exchange { Name = "Apple2", Id = 1 };

        // Act
        var result = await Repository.UpdateAsync(exchange);
        result.IsSuccess.Should().Be(true);

        // Assert
        var savedExchange = await Repository.GetByIdAsync(exchange.Id);
        savedExchange.Should().NotBeNull();
        savedExchange.Name.Should().Be("Apple2");
    }

    [Test]
    public async Task UpdateExchange_UpdateNotFound_ShouldFail()
    {
        // Arrange
        var exchange = new Exchange { Name = "Apple3", Id = 11111 };

        // Act
        Func<Task> saveAction = async () => { await Repository.UpdateAsync(exchange); };

        // Assert
        await saveAction.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task UpdateExchange_UpdateNegativeId_ShouldFail()
    {
        // Arrange
        var exchange = new Exchange { Name = "Apple3", Id = -11111 };
        // Act
        Func<Task> saveAction = async () => { await Repository.UpdateAsync(exchange); };

        // Assert
        await saveAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task UpdateExchange_UpdateNull_ShouldFail()
    {
        // Arrange
        Exchange exchange = null;

        // Act
        Func<Task> saveAction = async () => { await Repository.UpdateAsync(exchange); };
        await saveAction.Should().ThrowAsync<ArgumentNullException>();
    }

    #endregion

    [Test]
    public async Task DeleteExchange_ByExchange()
    {
        var exchange = new Exchange { Name = "Binance", Id = 1 };
        // Act
        var result = await Repository.DeleteAsync(exchange);
        result.IsSuccess.Should().Be(true);

        // Assert
        var savedExchange = await DbContext.Exchanges.FindAsync(exchange.Id);
        savedExchange.Should().BeNull();
    }

    [Test]
    public async Task DeleteExchange_ByExchangeId()
    {
        var excId = 2;
        // Act
        var result = await Repository.DeleteAsync(excId);
        result.IsSuccess.Should().Be(true);

        // Assert
        var savedExchange = await DbContext.Exchanges.FindAsync(excId);
        savedExchange.Should().BeNull();
    }

    [Test]
    public async Task DeleteExchange_DeleteNotExisting_ByExchangeId_ShouldFail()
    {
        var exchangeId = 11111;
        // Act
        Func<Task> saveAction = async () => { await Repository.DeleteAsync(exchangeId); };

        // Assert
        await saveAction.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DeleteExchange_DeleteNegativeId_ShouldFail()
    {
        var ExchangeId = -1;

        // Act
        Func<Task> saveAction = async () => { await Repository.DeleteAsync(ExchangeId); };

        // Assert
        await saveAction.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task DeleteExchange_DeleteNull_ShouldFail()
    {
        Exchange ExchangeId = null;

        // Act
        Func<Task> saveAction = async () => { await Repository.DeleteAsync(ExchangeId); };

        // Assert
        await saveAction.Should().ThrowAsync<ArgumentNullException>();
    }
}
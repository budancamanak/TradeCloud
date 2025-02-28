using Common.Core.Enums;
using FluentAssertions;
using FluentValidation;
using Market.Application.Features.GetPricesForPlugin.Request;
using Market.Application.Features.GetPricesForPlugin.Validator;
using Tests.Common.Data;

namespace Application.Tests.FeatureTests.GetPricesForPlugin;

[TestFixture]
public class GetPricesForPluginValidatorTests
{
    private GetPricesForPluginQueryValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new GetPricesForPluginQueryValidator();
    }

    [Test]
    public async Task GetPricesForPluginQueryValidator_OK()
    {
        var query = new GetPricesForPluginQuery(1,1, Timeframe.Day3.GetStringRepresentation(),
            MarketServiceTestData.Now, MarketServiceTestData.Now);
        var result = await _validator.ValidateAsync(query);
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Test]
    [TestCase("pl", -1, "test", "test")]
    [TestCase("pl", 1, "", "test")]
    [TestCase("pl", 0, "", "test")]
    [TestCase("pl", 1, "test", "")]
    [TestCase("", 1, "test", "")]
    [TestCase("a", 1, "test", "a")]
    [TestCase("", -1, "", "")]
    public async Task GetPricesForPluginQueryValidator_WhenMissingValueGiven_ShouldFail(string pluginId,
        int tickerId, string symbol, string exchangeName)
    {
        var query = new GetPricesForPluginQuery(-1,1, Timeframe.Day3.GetStringRepresentation(),
            MarketServiceTestData.Now, MarketServiceTestData.Now);

        Func<Task> action = async () => { await _validator.ValidateAndThrowAsync(query); };
        await action.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryValidator_WhenMissingStartDateGiven_ShouldFail()
    {
        var query = new GetPricesForPluginQuery(-1,1, Timeframe.Day3.GetStringRepresentation(),
            default, MarketServiceTestData.Now);

        Func<Task> action = async () => { await _validator.ValidateAndThrowAsync(query); };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Test]
    public async Task GetPricesForPluginQueryValidator_WhenMissingEndDateGiven_ShouldFail()
    {
        var query = new GetPricesForPluginQuery(-1,1, Timeframe.Day3.GetStringRepresentation(),
            MarketServiceTestData.Now, default);

        Func<Task> action = async () => { await _validator.ValidateAndThrowAsync(query); };
        await action.Should().ThrowAsync<ArgumentNullException>();
    }
}
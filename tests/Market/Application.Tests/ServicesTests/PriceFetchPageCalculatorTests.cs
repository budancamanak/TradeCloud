using Common.Core.Enums;
using FluentAssertions;
using Market.Application.Services;
using Tests.Common.Data;

namespace Application.Tests.ServicesTests;

[TestFixture]
public class PriceFetchPageCalculatorTests
{
    [OneTimeSetUp]
    public void Setup()
    {
    }

    [Test]
    public void PriceFetchPageCalculator_CalculatePages_UnixEpoch_OK()
    {
        var end = new DateTime(1970, 1, 2);
        var start = end.AddDays(-1);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, 2);
        pages.Should().NotBeNull();
        pages.Should().NotBeEmpty();
        pages.Count.Should().BeGreaterThanOrEqualTo(12);
    }

    [Test]
    public void PriceFetchPageCalculator_CalculatePages_NotIntersectingDates_OK()
    {
        var end = MarketServiceTestData.Instance.Prices.First().Timestamp;
        var start = end.AddDays(-1);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, 2);
        pages.Should().NotBeNull();
        pages.Should().NotBeEmpty();
        pages.Count.Should().BeGreaterThanOrEqualTo(12);
    }

    [Test]
    public void PriceFetchPageCalculator_CalculatePages_IntersectingDates_NoLaterThanEnd_OK()
    {
        var end = MarketServiceTestData.Instance.Prices.First().Timestamp.AddHours(2);
        var start = MarketServiceTestData.Instance.Prices.First().Timestamp.AddHours(-10);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, 2);
        pages.Should().NotBeNull();
        pages.Should().NotBeEmpty();
        pages.Count.Should().BeGreaterThanOrEqualTo(6);
    }

    [Test]
    public void PriceFetchPageCalculator_CalculatePages_IntersectingDates_NoEarlierThanStart_OK()
    {
        var end = MarketServiceTestData.Instance.Prices.Last().Timestamp.AddHours(8);
        var start = MarketServiceTestData.Instance.Prices.Last().Timestamp.AddHours(-12);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, 2);
        pages.Should().NotBeNull();
        pages.Should().NotBeEmpty();
        pages.Count.Should().BeGreaterThanOrEqualTo(10);
    }

    [Test]
    public void PriceFetchPageCalculator_CalculatePages_IntersectingDates_EachSide_OK()
    {
        var end = MarketServiceTestData.Instance.Prices.Last().Timestamp.AddHours(8);
        var start = MarketServiceTestData.Instance.Prices.Last().Timestamp.AddHours(-16);
        var pages = PriceFetchPageCalculator.ToPages(Timeframe.Hour1, start, end, 2);
        pages.Should().NotBeNull();
        pages.Should().NotBeEmpty();
        pages.Count.Should().BeGreaterThanOrEqualTo(12);
    }

    [Test]
    public void PriceFetchPageCalculator_NullPriceList_ShouldFail()
    {
        Action getAction = () => { PriceFetchPageCalculator.ToPages(Timeframe.Day1, default, default, 100); };

        // Assert
        getAction.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void PriceFetchPageCalculator_NullStartDate_ShouldFail()
    {
        Action getAction = () => { PriceFetchPageCalculator.ToPages(Timeframe.Day1, default, default, 100); };

        // Assert
        getAction.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void PriceFetchPageCalculator_NullEndDate_ShouldFail()
    {
        Action getAction = () =>
        {
            PriceFetchPageCalculator.ToPages(Timeframe.Day1, default, MarketServiceTestData.Now, 100);
        };

        // Assert
        getAction.Should().Throw<ArgumentNullException>();
    }
}
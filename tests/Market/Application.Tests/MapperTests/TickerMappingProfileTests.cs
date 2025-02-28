using AutoMapper;
using Common.Core.DTOs;
using FluentAssertions;
using Market.Application.Mappers;
using Tests.Common;
using Tests.Common.Data;

namespace Application.Tests.MapperTests;

[TestFixture]
public class TickerMappingProfileTests : AbstractLoggableTest
{
    private IMapper _mapper;

    [SetUp]
    public void SetUp()
    {
        base.SetUp();
        var mappingConfig = new MapperConfiguration(mc => { mc.AddProfile(new TickerMappingProfile()); });
        _mapper = mappingConfig.CreateMapper();
    }

    [Test]
    public void TickerMappingProfile_MapTicker_ToDto()
    {
        var ticker = MarketServiceTestData.Instance.Tickers[0];
        var mapped = _mapper.Map<TickerDto>(ticker);
        mapped.Should().NotBeNull();
        mapped.Name.Should().Be(ticker.Name);
        mapped.ExchangeName.Should().Be(ticker.Exchange.Name);
    }
}
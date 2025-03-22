using Common.Plugin.Models;
using FluentAssertions;
using Newtonsoft.Json;
using Worker.Plugins.MovingAverage;

namespace Infrastructure.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var paramSet = new GoldenDeathCrossPluginParams(50, 200);
        var str = JsonConvert.SerializeObject(paramSet);
        var returned = JsonConvert.DeserializeObject<GoldenDeathCrossPluginParams>(str);
        returned.Should().NotBeNull();
        returned.FastMovingAverage.Should().Be(50);
        returned.SlowMovingAverage.Should().Be(200);
        Console.WriteLine(paramSet.GetParamSet().ToJson());
        Console.WriteLine(str);
        Console.WriteLine(returned);
        Console.WriteLine(returned.FastMovingAverage.ToString());
    }
}
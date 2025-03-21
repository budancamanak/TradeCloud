using Common.Plugin.Models;
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
        Console.WriteLine(str);
        Console.WriteLine(returned);
        Console.WriteLine(returned.FastMovingAverage.ToString());
        Assert.Pass();
    }
}
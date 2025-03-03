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
        var paramSet = new GoldenDeathCrossPluginParamSet(50, 200);
        var str = JsonConvert.SerializeObject(paramSet);
        var returned = JsonConvert.DeserializeObject<GoldenDeathCrossPluginParamSet>(str);
        Console.WriteLine(str);
        Console.WriteLine(returned);
        Console.WriteLine(returned.FastMovingAverage.ToString());
        Assert.Pass();
    }

    [Test]
    public void Test2()
    {
        var paramSet = ListParameter<int>.IntParameter("Test String parameter", 0, 11, 12, 13, 14, 15);
        var str = JsonConvert.SerializeObject(paramSet);
        Console.WriteLine(str);
        var returned = JsonConvert.DeserializeObject<ListParameter<int>>(str);
        Console.WriteLine(returned.Name);
        Console.WriteLine(returned.Items.Count);
        Assert.AreEqual(paramSet.Items.Count, returned.Items.Count);
    }
}
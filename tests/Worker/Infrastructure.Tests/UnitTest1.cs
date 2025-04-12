using System.Linq.Expressions;
using System.Text;
using Common.Plugin.Models;
using FluentAssertions;
using Newtonsoft.Json;
using StackExchange.Redis;
using Worker.Plugins.MovingAverage;

namespace Infrastructure.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    protected TOut CachedFunc<TOut>(Func<TOut> func)
    {
        Console.WriteLine("Called CachedFunc on . {}", func.ToString());
        Console.WriteLine(">> func.Target : {}", func.Target?.ToString());
        Console.WriteLine(">> func.Method : {}", func.Method.ToString());
        return func();
    }

    protected TOut CachedFunc2<TOut>(Expression<TOut> func)
    {
        var x = func.Compile();
        return x;
    }


    private int Multiply(int x, int y)
    {
        return x * y;
    }

    private string GenerateCacheKey(string prefix, params object[]? args)
    {
        StringBuilder sb = new StringBuilder(prefix);
        if (args != null)
        {
            sb.Append("_");
            foreach (var value in args)
            {
                sb.Append(value).Append("_");
            }
        }

        return sb.ToString();
    }

    [Test]
    public void TestFunc()
    {
        // var x = CachedFunc2(() => Multiply(3, 5));
        // var f = () => Multiply(3, 5);
        // Expression<Func<int>> bind = () => f();
        // var a = bind.Compile().Invoke();
        // var x = CachedFunc2(bind).Invoke();
        string key = GenerateCacheKey("PREF", 1, 3, 4, null, 6.0);
        Console.WriteLine(key);
        var x = 3;
        Assert.IsTrue(x == 3);
    }

    [Test]
    [Ignore("skip for fast")]
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
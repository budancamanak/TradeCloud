using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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

    // private TOut CachedFunc3<TOut>(string cacheKey, Func<TOut> func)
    // {
    //     var cached = cache.GetAsync<TOut>(cacheKey).GetAwaiter().GetResult();
    //     if (cached != null) return cached;
    //     cached = func();
    //     cache.SetAsync(cacheKey, cached, TimeSpan.FromMinutes(30)).GetAwaiter().GetResult();
    //     return cached;
    // }

    private int Multiply(int x, int y)
    {
        return x * y;
    }

    private int exeId = 3;

    private string GenerateCacheKey(object[]? args = null, [CallerMemberName] string callerName = "_caller_method_")
    {
        StringBuilder sb = new StringBuilder(callerName);
        sb.Append("_").Append(exeId);
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

    protected static object[] Args(params object[] args)
    {
        return args;
    }


    [Test]
    public void TestFunc()
    {
        // var x = CachedFunc2(() => Multiply(3, 5));
        // var f = () => Multiply(3, 5);
        // Expression<Func<int>> bind = () => f();
        // var a = bind.Compile().Invoke();
        // var x = CachedFunc2(bind).Invoke();
        string key = GenerateCacheKey(Args(1, 3, 4, null, 6.0));
        Console.WriteLine(key);
        key.Should().Be("TestFunc_3_1_3_4__6_");
    }
    [Test]
    public void Test_GetSma()
    {
        // var x = CachedFunc2(() => Multiply(3, 5));
        // var f = () => Multiply(3, 5);
        // Expression<Func<int>> bind = () => f();
        // var a = bind.Compile().Invoke();
        // var x = CachedFunc2(bind).Invoke();
        string key = GenerateCacheKey(Args(1, 3, 4, null, 6.1));
        Console.WriteLine(key);
        key.Should().Be("Test_GetSma_3_1_3_4__6,1_");
    }
    [Test]
    public void Test_GetSma_Null()
    {
        string key = GenerateCacheKey();
        Console.WriteLine(key);
        key.Should().Be("Test_GetSma_Null_3");
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
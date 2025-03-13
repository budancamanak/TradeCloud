using Backend.Domain.Entities;
using Backend.Infrastructure.Services;

namespace Application.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var engine = new PluginExecutionEngine();
        var exe = new AnalysisExecution();
        // {"FastMovingAverage":{"Value":50,"Min":50,"Max":50,"Name":"FastMovingAverage","Increment":0},"SlowMovingAverage":{"Value":200,"Min":200,"Max":200,"Name":"SlowMovingAverage","Increment":0}}
        // {"FastMovingAverage":{"Value":50,"Min":50,"Max":50,"Name":"FastMovingAverage","Increment":0},"SlowMovingAverage":{"Value":200,"Min":200,"Max":200,"Name":"SlowMovingAverage","Increment":0}}
        exe.ParamSet =
            "{\"FastMovingAverage\":{\"Value\":50,\"Min\":50,\"Max\":50,\"Name\":\"FastMovingAverage\",\"Increment\":0},\"SlowMovingAverage\":{\"Value\":200,\"Min\":200,\"Max\":200,\"Name\":\"SlowMovingAverage\",\"Increment\":0},\"MaTypes\":{\"DefaultIndex\":0,\"Items\":[\"SMA\",\"EMA\",\"WMA\"],\"Name\":\"SlowMovingAverage\"},\"MaTypes2\":{\"DefaultIndex\":0,\"Items\":[1,2,3],\"Name\":\"SlowMovingAverage\"}}";
        // exe.ParamSet =
        //     "{\"FastMovingAverage\":{\"Value\":50,\"Min\":50,\"Max\":50,\"Name\":\"FastMovingAverage\",\"Increment\":0},\"SlowMovingAverage\":{\"Value\":200,\"Min\":200,\"Max\":200,\"Name\":\"SlowMovingAverage\",\"Increment\":0}}";
        var mr = engine.GeneratePluginExecutions(exe).GetAwaiter().GetResult();
        Assert.Pass();
    }
}
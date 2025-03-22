using Backend.Domain.Entities;
using Backend.Infrastructure.Services;
using Common.Plugin.Models;
using Newtonsoft.Json;

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
        // exe.ParamSet = "{\"Name\":\"FastMovAvg\",\"Type\":0,\"Range\":0,\"Value\":25}";
        // exe.ParamSet = "{\"Name\":\"FastMovRange\",\"Type\":0,\"Range\":1,\"Value\":{\"Min\":15,\"Max\":50,\"Increment\":1,\"Default\":20}}";
        // exe.ParamSet ="{\"Name\":\"fastMovList\",\"Type\":0,\"Range\":2,\"Value\":{\"Items\":[15,20,25],\"DefaultIndex\":0}}";
        exe.ParamSet =
            "[\n{\"Name\":\"FastMovAvg\",\"Type\":0,\"Range\":0,\"Value\":25},\n{\"Name\":\"FastMovRange\",\"Type\":0,\"Range\":1,\"Value\":{\"Min\":15,\"Max\":50,\"Increment\":1,\"Default\":20}},\n{\"Name\":\"fastMovList\",\"Type\":0,\"Range\":2,\"Value\":{\"Items\":[15,20,25],\"DefaultIndex\":0}}\n]";
        var fastMov = Param.Int.Single("FastMovAvg", 25);
        var fastMovRange = Param.Int.Range("FastMovRange", 15, 50, 1, 20);
        var fastMovList = Param.Int.List("fastMovList", 0, 15, 20, 25);
        //
        // var js = JsonConvert.SerializeObject(fastMov);
        // Console.WriteLine(js);
        // var js2 = JsonConvert.SerializeObject(fastMovRange);
        // Console.WriteLine(js2);
        // var js3 = JsonConvert.SerializeObject(fastMovList);
        // Console.WriteLine(js3);

        // {"FastMovingAverage":{"Value":50,"Min":50,"Max":50,"Name":"FastMovingAverage","Increment":0},"SlowMovingAverage":{"Value":200,"Min":200,"Max":200,"Name":"SlowMovingAverage","Increment":0}}
        // {"FastMovingAverage":{"Value":50,"Min":50,"Max":50,"Name":"FastMovingAverage","Increment":0},"SlowMovingAverage":{"Value":200,"Min":200,"Max":200,"Name":"SlowMovingAverage","Increment":0}}
        // exe.ParamSet =
        //     "{\"FastMovingAverage\":{\"Value\":50,\"Min\":50,\"Max\":50,\"Name\":\"FastMovingAverage\",\"Increment\":0},\"SlowMovingAverage\":{\"Value\":200,\"Min\":200,\"Max\":200,\"Name\":\"SlowMovingAverage\",\"Increment\":0},\"MaTypes\":{\"DefaultIndex\":0,\"Items\":[\"SMA\",\"EMA\",\"WMA\"],\"Name\":\"SlowMovingAverage\"},\"MaTypes2\":{\"DefaultIndex\":0,\"Items\":[1,2,3],\"Name\":\"SlowMovingAverage\"}}";
        // exe.ParamSet =
        //     "{\"FastMovingAverage\":{\"Value\":50,\"Min\":50,\"Max\":50,\"Name\":\"FastMovingAverage\",\"Increment\":0},\"SlowMovingAverage\":{\"Value\":200,\"Min\":200,\"Max\":200,\"Name\":\"SlowMovingAverage\",\"Increment\":0}}";
        // var mr = engine.GeneratePluginExecutions(exe).GetAwaiter().GetResult();
        Console.WriteLine(exe.ParamSet);
        var mr = engine.GenerateParameters(exe);
        var plugins = engine.GeneratePluginExecutions(exe);
        foreach (var item in plugins)
        {
            var paramset = TestParamSet.ParseParams(item.ParamSet);
            // Console.WriteLine(paramset);
        }

        Assert.Pass();
    }
}
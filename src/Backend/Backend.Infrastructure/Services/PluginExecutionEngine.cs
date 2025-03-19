using Ardalis.GuardClauses;
using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend.Infrastructure.Services;

public class PluginExecutionEngine : IPluginExecutionEngine
{
    static List<List<Param>> Cartesian(List<List<Param>> sets)
    {
        List<List<Param>> temp = new List<List<Param>> { new List<Param>() };
        for (int i = 0; i < sets.Count; i++)
        {
            List<List<Param>> newTemp = new List<List<Param>>();
            foreach (List<Param> product in temp)
            {
                foreach (Param element in sets[i])
                {
                    List<Param> tempCopy = new List<Param>(product);
                    tempCopy.Add(element);
                    newTemp.Add(tempCopy);
                }
            }

            temp = newTemp;
        }

        foreach (List<Param> product in temp)
        {
            Console.WriteLine(string.Join(" ", product));
        }

        return temp;
    }

    public List<PluginExecution> GeneratePluginExecutions(AnalysisExecution execution)
    {
        var list = new List<PluginExecution>();
        var parameters = GenerateParameters(execution);
        List<List<Param>> deflated = new List<List<Param>>();
        foreach (var item in parameters)
        {
            deflated.Add(item.Deflate());
        }

        var cartesian = Cartesian(deflated);
        foreach (var param in cartesian)
        {
            var dict = new Dictionary<string, object>();
            foreach (var item in param)
            {
                dict.Add(item.Name, item.Value);
            }

            var plugin = new PluginExecution
            {
                ParamSet = JsonConvert.SerializeObject(dict),
                AnalysisExecutionId = execution.Id,
                Status = PluginStatus.Init,
                Error = "",
                Progress = 0
            };
            list.Add(plugin);
        }

        return list;
    }

    public List<Param> GenerateParameters(AnalysisExecution execution)
    {
        var listOfParams = new List<Param>();

        var parameters = JsonConvert.DeserializeObject<Param[]>(execution.ParamSet);
        Guard.Against.NullOrZeroLengthArray(parameters);
        foreach (var param in parameters!)
        {
            switch (param.Type)
            {
                case ParameterType.Int:
                    ParseIntParamValue(param);
                    break;
                case ParameterType.Double:
                    ParseDoubleParamValue(param);
                    break;
                case ParameterType.Str:
                    ParseStringParamValue(param);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }


            Console.WriteLine(param);
            listOfParams.Add(param);
        }

        return listOfParams;
    }

    private static void ParseStringParamValue(Param param)
    {
        switch (param.Range)
        {
            case ParameterRange.Single:
                param.Value = param.Value.ToString();
                break;
            case ParameterRange.List:
                param.Value = JsonConvert.DeserializeObject<StringListValue>(param.Value.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ParseDoubleParamValue(Param param)
    {
        switch (param.Range)
        {
            case ParameterRange.Single:
                param.Value = double.Parse(param.Value.ToString());
                break;
            case ParameterRange.Range:
                param.Value = JsonConvert.DeserializeObject<DoubleParamValue>(param.Value.ToString());
                break;
            case ParameterRange.List:
                param.Value = JsonConvert.DeserializeObject<DoubleListValue>(param.Value.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static void ParseIntParamValue(Param param)
    {
        switch (param.Range)
        {
            case ParameterRange.Single:
                param.Value = int.Parse(param.Value.ToString());
                break;
            case ParameterRange.Range:
                param.Value = JsonConvert.DeserializeObject<IntParamValue>(param.Value.ToString());
                break;
            case ParameterRange.List:
                param.Value = JsonConvert.DeserializeObject<IntListValue>(param.Value.ToString());
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
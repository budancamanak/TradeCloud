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
    public List<PluginExecution> GeneratePluginExecutions(AnalysisExecution execution)
    {
        var list = new List<PluginExecution>();
        var parameters = GenerateParameters(execution);
        foreach (var param in parameters)
        {
            var plugin = new PluginExecution
            {
                ParamSet = JsonConvert.SerializeObject(param),
                AnalysisExecutionId = execution.Id,
                Status = PluginStatus.Init,
                Error = "",
                Progress = 0
            };
            list.Add(plugin);
        }

        return list;
    }

    public List<Param> CartesianProductParameters(List<Param> original)
    {
        var listOfParams = new List<Param>();
        return listOfParams;
    }

    public List<Param> DeflateParameters(Param param)
    {
        var listOfParams = new List<Param>();
        switch (param.Type)
        {
            case ParameterType.Int:
                switch (param.Range)
                {
                    case ParameterRange.Single:
                        break;
                    case ParameterRange.Range:
                        var v = param.Value as IntParamValue;
                        var def = v.Deflate();
                        foreach (var item in def)
                        {
                            listOfParams.Add(new Param
                            {
                                Type = param.Type,
                                Range = ParameterRange.Single,
                                Value = item,
                                Name = param.Name
                            });
                        }

                        break;
                    case ParameterRange.List:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

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

        return listOfParams;
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
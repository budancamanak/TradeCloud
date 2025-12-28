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
    private const int MaxCartesianSize = 1000;

    static IEnumerable<List<Param>> CartesianLazy(List<List<Param>> sets)
    {
        if (sets.Count == 0)
        {
            yield return new List<Param>();
            yield break;
        }

        var indices = new int[sets.Count];
        var lengths = sets.Select(s => s.Count).ToArray();

        while (true)
        {
            // Build current combination
            var result = new List<Param>(sets.Count);
            for (int i = 0; i < sets.Count; i++)
            {
                result.Add(sets[i][indices[i]]);
            }

            yield return result;

            // Increment indices (like counting in mixed-radix)
            int pos = sets.Count - 1;
            while (pos >= 0)
            {
                indices[pos]++;
                if (indices[pos] < lengths[pos])
                    break;
                indices[pos] = 0;
                pos--;
            }

            if (pos < 0) yield break;
        }
    }

    private static int PossibleCombinationsCount(List<List<Param>> sets)
    {
        var count = 1;
        foreach (var set in sets)
        {
            count *= Math.Max(1, set.Count);
            if (count > MaxCartesianSize)
                return count;
        }

        return count;
    }


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

    public List<PluginExecution> GeneratePluginExecutionsLazy(AnalysisExecution execution)
    {
        var parameters = GenerateParameters(execution);
        var deflated = parameters.Select(p => p.Deflate()).ToList();
        var estimatedSize = PossibleCombinationsCount(deflated);
        if (estimatedSize > MaxCartesianSize)
        {
            throw new InvalidOperationException(
                $"Parameter combination count ({(estimatedSize):N0}) exceeds maximum allowed ({MaxCartesianSize:N0}). " +
                "Please reduce parameter ranges.");
        }

        Console.WriteLine($"[PluginExecutionEngine] Generating {estimatedSize} plugin executions lazily.");
        var list = new List<PluginExecution>();
        foreach (var param in CartesianLazy(deflated))
        {
            var dict = param.ToDictionary(p => p.Name, p => p.Value);
            list.Add(new PluginExecution
            {
                ParamSet = JsonConvert.SerializeObject(dict),
                AnalysisExecutionId = execution.Id,
                Status = PluginStatus.Init,
                Error = "",
                Progress = 0
            });
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
            if (string.IsNullOrWhiteSpace(param.Name)) continue;
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


            // Console.WriteLine(param);
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
                var arr = ((JArray)param.Value);
                param.Value = new StringListValue
                {
                    Items = arr.Values<string>().ToArray()
                };
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
                var arr = ((JArray)param.Value);
                param.Value = new DoubleListValue()
                {
                    Items = arr.Values<double>().ToArray()
                };
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
                var arr = ((JArray)param.Value);
                param.Value = new IntListValue()
                {
                    Items = arr.Values<int>().ToArray()
                };
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
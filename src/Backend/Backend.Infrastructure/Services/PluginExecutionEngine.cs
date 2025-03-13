using Backend.Application.Abstraction.Services;
using Backend.Domain.Entities;
using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Backend.Infrastructure.Services;

public class PluginExecutionEngine : IPluginExecutionEngine
{
    public Task<List<PluginExecution>> GeneratePluginExecutions(AnalysisExecution execution)
    {
        var listOfParams = new List<BaseParameter>();
        IDictionary<string, JToken> jsondata = JObject.Parse(execution.ParamSet);
        foreach (var element in jsondata)
        {
            if (element.Value["Items"] != null)
            {
                if (element.Value["Items"] is not JArray arr) continue;
                switch (arr[0].Type)
                {
                    case JTokenType.Float:
                    {
                        var listParam = JsonConvert.DeserializeObject<ListParameter<double>>(element.Value.ToString());
                        listOfParams.Add(listParam);
                        Console.WriteLine(listParam);
                        break;
                    }
                    case JTokenType.Integer:
                    {
                        var listParam = JsonConvert.DeserializeObject<ListParameter<int>>(element.Value.ToString());
                        listOfParams.Add(listParam);
                        Console.WriteLine(listParam);
                        break;
                    }
                    default:
                    {
                        var listParam = JsonConvert.DeserializeObject<ListParameter<string>>(element.Value.ToString());
                        listOfParams.Add(listParam);
                        Console.WriteLine(listParam);
                        break;
                    }
                }
            }
            else
            {
                var min = element.Value["Min"];
                if (min != null)
                {
                    switch (min.Type)
                    {
                        case JTokenType.Integer:
                            var numericIntParam =
                                JsonConvert.DeserializeObject<NumericParameter<int>>(element.Value.ToString());
                            Console.WriteLine(numericIntParam);
                            listOfParams.Add(numericIntParam);
                            break;
                        case JTokenType.Float:
                            var numericDoubleParam =
                                JsonConvert.DeserializeObject<NumericParameter<float>>(element.Value.ToString());
                            Console.WriteLine(numericDoubleParam);
                            listOfParams.Add(numericDoubleParam);
                            break;
                        default:
                            throw new Exception("Failed to parse json");
                    }
                }
            }
        }

        Console.WriteLine("hry");
        foreach (var paramset in listOfParams)
        {
            paramset

            Console.WriteLine(paramset);
        }

        throw new NotImplementedException();
    }

    public List<IPluginParamSet> GenerateParameters(AnalysisExecution execution)
    {
        throw new NotImplementedException();
    }
}
using Common.Plugin.Abstraction;
using Common.Plugin.Models;
using Newtonsoft.Json;

namespace Application.Tests;

public class TestParamSet : IPluginParamSet
{
    public int FastMovAvg { get; set; }
    public int FastMovRange { get; set; }
    public int fastMovList { get; set; }
    
    /***
     * IParameters -> List of values to be used within a plugin. properties are primitive
     *                  Will return IParamSet as well.
     * IParamSet -> List of Param objects. Will be used to provide boundaries to ui. is related to AnalysisExecution
     * Param -> An object that holds parameter info. Has int,string,double types and range,list,single structures
     */
    
    /***
     * ParamSet -> A class that holds an array of param
     */
    
    public TestParamSet()
    {
        
    }

    public string GetStringRepresentation()
    {
        throw new NotImplementedException();
    }

    public string ToJson()
    {
        throw new NotImplementedException();
    }

    public static TestParamSet ParseParams(string? json)
    {
        try
        {
            return JsonConvert.DeserializeObject<TestParamSet>(json);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return null;
    }
}
using Newtonsoft.Json;

namespace Common.Core.Serialization;

public class JsonSerialization
{
    public static string ToJson(object obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects
        });
    }
}
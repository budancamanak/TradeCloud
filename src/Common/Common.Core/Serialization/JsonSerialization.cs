using Newtonsoft.Json;

namespace Common.Core.Serialization;

public class JsonSerialization
{
    public static string ToJson<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        });
    }

    public static T? FromJson<T>(string data)
    {
        return JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            StringEscapeHandling = StringEscapeHandling.EscapeNonAscii
        });
    }
}
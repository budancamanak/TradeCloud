using System.Numerics;

namespace Common.Plugin.Abstraction;

public interface IPluginParameter<T> where T : ISignedNumber<T>, IComparisonOperators<T, T, bool>
{
    T GetParameterValue();
    string GetParameterName();
    T GetMinValue();
    T GetMaxValue();
    T GetIncrement();

    IEnumerator<T> GetEnumerator();
    string ToJson();
    void FromJson(string json);
}
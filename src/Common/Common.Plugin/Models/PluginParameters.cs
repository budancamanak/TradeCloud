using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Ardalis.GuardClauses;
using Common.Core.Extensions;

namespace Common.Plugin.Models;

public enum ParameterType
{
    Int,
    Double,
    Str
}

public enum ParameterRange
{
    Single,
    Range,
    List
}

public abstract class ParamValue<T>
{
    public abstract List<T> Deflate();
}

public class IntParamValue : ParamValue<int>
{
    public int Min { get; set; }
    public int Max { get; set; }
    public int Increment { get; set; }
    public int Default { get; set; }

    public override List<int> Deflate()
    {
        var list = new List<int>();
        for (int i = Min; i <= Max; i += Increment)
        {
            list.Add(i);
        }

        return list;
    }
}

public class IntListValue : ParamValue<int>
{
    public int[] Items { get; set; }
    public int DefaultIndex { get; set; }

    public override List<int> Deflate()
    {
        return Items.ToList();
    }
}

public class DoubleListValue : ParamValue<double>
{
    public double[] Items { get; set; }
    public int DefaultIndex { get; set; }

    public override List<double> Deflate()
    {
        return Items.ToList();
    }
}

public class DoubleParamValue : ParamValue<double>
{
    public double Min { get; set; }
    public double Max { get; set; }
    public double Increment { get; set; }
    public double Default { get; set; }

    public override List<double> Deflate()
    {
        var list = new List<double>();
        for (var i = Min; i < Max; i += Increment)
        {
            list.Add(i);
        }

        return list;
    }
}

public class StringListValue : ParamValue<string>
{
    public string[] Items { get; set; }
    public int DefaultIndex { get; set; }

    public override List<string> Deflate()
    {
        return Items.ToList();
    }
}

public class Param
{
    public string Name { get; set; } = "";
    public ParameterType Type { get; set; }
    public ParameterRange Range { get; set; }
    public object Value { get; set; }

    public Param(string name, ParameterType type, ParameterRange range, object value)
    {
        Name = name;
        Type = type;
        Range = range;
        Value = value;
    }

    public List<Param> Deflate()
    {
        var listOfParams = new List<Param>();
        switch (Type)
        {
            case ParameterType.Int:
                switch (Range)
                {
                    case ParameterRange.Single:
                        listOfParams.Add(new Param(Name, Type, ParameterRange.Single, int.Parse(Value.ToString())));
                        break;
                    case ParameterRange.Range:
                        var v = Value as IntParamValue;
                        var def = v.Deflate();
                        foreach (var item in def)
                        {
                            listOfParams.Add(new Param(Name, Type, ParameterRange.Single, item));
                        }

                        break;
                    case ParameterRange.List:
                        var vl = Value as IntListValue;
                        var defl = vl.Deflate();
                        foreach (var item in defl)
                        {
                            listOfParams.Add(new Param(Name, Type, ParameterRange.Single, item));
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                break;
            case ParameterType.Double:
                break;
            case ParameterType.Str:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return listOfParams;
    }

    public static int operator !(Param left)
    {
        return left.AsInt();
    }

    public static double operator +(Param left)
    {
        return left.AsDouble();
    }

    public static string operator ~(Param left)
    {
        return left.AsString();
    }

    private int AsInt()
    {
        return int.TryParse(Value.ToString(), out var intValue)
            ? intValue
            : throw new ArgumentException($"Failed to convert value to int. Value:{Value}");
    }

    private double AsDouble()
    {
        return double.TryParse(Value.ToString(), out var intValue)
            ? intValue
            : throw new ArgumentException($"Failed to convert value to double. Value:{Value}");
        ;
    }

    private string AsString()
    {
        var v = Value?.ToString();
        return !string.IsNullOrWhiteSpace(v)
            ? v
            : throw new ArgumentException($"Failed to convert value to int. Value:{Value}");
    }

    public override string ToString()
    {
        return $"Name: {Name} Type: {Type} Range: {Range} Value: {Value}";
    }

    public static class Int
    {
        public static Param Single(string name, int value)
        {
            return new Param(name, ParameterType.Int, ParameterRange.Single, value);
        }

        public static Param Range(string name, int min, int max, int inc, int def)
        {
            return new Param(name, ParameterType.Int, ParameterRange.Range,
                new IntParamValue
                {
                    Increment = inc,
                    Default = def,
                    Min = min,
                    Max = max
                }
            );
        }

        public static Param List(string name, int defIndex, params int[] items)
        {
            return new Param(name, ParameterType.Int, ParameterRange.List,
                new IntListValue
                {
                    DefaultIndex = defIndex,
                    Items = items
                }
            );
        }
    }
}
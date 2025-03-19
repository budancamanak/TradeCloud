using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Ardalis.GuardClauses;
using Common.Core.Extensions;

namespace Common.Plugin.Models;

public abstract class BaseParameter
{
    public abstract List<object> GetItems();
    public abstract int GetDefaultIndex();
    public abstract string GetName();
    public abstract double GetMin();
    public abstract double GetMax();
    public abstract double GetValue();
    public abstract double GetIncrement();
}

public class ListParameter<T> : BaseParameter
{
    public ListParameter(string name, int defaultIndex, params T[] items)
    {
        this.Name = name;
        this.DefaultIndex = defaultIndex;
        Guard.Against.NullOrZeroLengthArray(items);
        Items = items.ToList();
    }

    public int DefaultIndex { get; set; }
    public List<T> Items { get; set; }
    public string Name { get; set; }

    public IEnumerator<T> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    public static ListParameter<int> IntParameter(string name, int defaultIndex, params int[] args)
    {
        return new ListParameter<int>(name, defaultIndex, args);
    }

    public static ListParameter<double> DoubleParameter(string name, int defaultIndex, params double[] args)
    {
        return new ListParameter<double>(name, defaultIndex, args);
    }

    public static ListParameter<string> StringParameter(string name, int defaultIndex, params string[] args)
    {
        return new ListParameter<string>(name, defaultIndex, args);
    }

    public override List<object> GetItems()
    {
        return Items as List<object>;
    }

    public override int GetDefaultIndex()
    {
        return DefaultIndex;
    }

    public override string GetName()
    {
        return Name;
    }

    public override double GetMin()
    {
        throw new NotImplementedException();
    }

    public override double GetMax()
    {
        throw new NotImplementedException();
    }

    public override double GetValue()
    {
        throw new NotImplementedException();
    }

    public override double GetIncrement()
    {
        throw new NotImplementedException();
    }
}

public class NumericParameter<T> : BaseParameter where T : ISignedNumber<T>, IComparisonOperators<T, T, bool>
{
    public NumericParameter(string name, T value, T min, T max, T increment)
    {
        this.Min = min;
        this.Max = max;
        this.Increment = increment;
        this.Value = value;
        this.Name = name;
    }

    public T Value { get; set; }
    public T Min { get; set; }
    public T Max { get; set; }
    public string Name { get; set; }
    public T Increment { get; set; }

    public IEnumerator<T> GetEnumerator()
    {
        var finished = false;
        var lastVal = Min;
        do
        {
            yield return lastVal;
            lastVal += Increment;
            finished = lastVal < Max;
        } while (!finished);
    }

    public static T operator !(NumericParameter<T> left)
    {
        return left.Value;
    }

    public static NumericParameter<int> IntParameter(string name, int value)
    {
        return new NumericParameter<int>(name, value, value, value, 0);
    }

    public static NumericParameter<int> IntParameter(string name, int value, int min, int max, int inc)
    {
        return new NumericParameter<int>(name, value, min, max, inc);
    }

    public static NumericParameter<double> IntParameter(string name, double value)
    {
        return new NumericParameter<double>(name, value, value, value, 0);
    }

    public static NumericParameter<double> IntParameter(string name, double value, double min, double max, double inc)
    {
        return new NumericParameter<double>(name, value, min, max, inc);
    }

    public override string ToString()
    {
        return $"{Name}:{Value}";
    }

    public override List<object> GetItems()
    {
        throw new NotImplementedException();
    }

    public override int GetDefaultIndex()
    {
        throw new NotImplementedException();
    }

    public override string GetName()
    {
        return Name;
    }

    public override double GetMin()
    {
        throw new NotImplementedException();
    }

    public override double GetMax()
    {
        throw new NotImplementedException();
    }

    public override double GetValue()
    {
        throw new NotImplementedException();
    }

    public override double GetIncrement()
    {
        throw new NotImplementedException();
    }
}

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
        for (int i = Min; i < Max; i += Increment)
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

    public override string ToString()
    {
        return $"{Name} {Type} {Range} {Value}";
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
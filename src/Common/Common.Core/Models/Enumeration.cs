using System.Reflection;

namespace Common.Core.Models;

public abstract class Enumeration<T> : IEquatable<Enumeration<T>>
    where T : Enumeration<T>
{
    private static readonly Dictionary<int, T> _enumerations = CreateEnumerations();
    public int Value { get; protected init; }
    public string Name { get; protected init; } = string.Empty;

    protected Enumeration(int value, string name)
    {
        this.Value = value;
        this.Name = name;
    }

    public static T? FromValue(int value)
    {
        return _enumerations.TryGetValue(value, out var v) ? v : default;
    }

    public static T? FromName(string name)
    {
        return _enumerations.Values.SingleOrDefault(f => f.Name == name);
    }

    public bool Equals(Enumeration<T>? other)
    {
        if (other == null) return false;
        return GetType() == other.GetType() && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is Enumeration<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    private static Dictionary<int, T> CreateEnumerations()
    {
        var _type = typeof(T);
        var fields = _type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => _type.IsAssignableFrom(f.FieldType))
            .Select(s => (T)s.GetValue(default)!);
        return fields.ToDictionary(f => f.Value);
    }
}
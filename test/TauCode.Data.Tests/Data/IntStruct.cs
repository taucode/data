namespace TauCode.Data.Tests.Data;

public readonly struct IntStruct : IComparable<IntStruct>, IComparable, IEquatable<IntStruct>
{
    public IntStruct(int value)
    {
        Value = value;
    }

    public readonly int Value;

    public int CompareTo(IntStruct other)
    {
        return this.Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is IntStruct other)
        {
            return this.Value.CompareTo(other.Value);
        }

        throw new ArgumentException();
    }

    public bool Equals(IntStruct other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is IntStruct other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value;
    }

    public static implicit operator IntStruct?(int? v)
    {
        if (v.HasValue)
        {
            return new IntStruct(v.Value);
        }

        return null;
    }

    public static implicit operator IntStruct(int v)
    {
        return new IntStruct(v);
    }

    public override string ToString() => this.Value.ToString();
}

namespace TauCode.Data.Tests.Data;

public class IntClass : IComparable<IntClass>, IComparable, IEquatable<IntClass>
{
    public IntClass()
    {
    }

    public IntClass(int value)
    {
        Value = value;
    }

    public readonly int Value;

    public int CompareTo(IntClass? other)
    {
        if (other == null)
        {
            throw new ArgumentNullException(nameof(other));
        }

        return this.Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is IntClass other)
        {
            return this.Value.CompareTo(other.Value);
        }

        throw new ArgumentException();
    }

    public static implicit operator IntClass?(int? v)
    {
        if (v.HasValue)
        {
            return new IntClass(v.Value);
        }

        return null;
    }

    public bool Equals(IntClass? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (obj is int n)
        {
            return this.Value == n;
        }

        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((IntClass)obj);
    }

    public override int GetHashCode()
    {
        return Value;
    }

    public override string ToString() => this.Value.ToString();
}

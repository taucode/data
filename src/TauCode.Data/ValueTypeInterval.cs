using System.Text;

namespace TauCode.Data;

public readonly struct ValueTypeInterval<T> : IEquatable<ValueTypeInterval<T>>
    where T : struct, IComparable<T>
{
    #region Fields

    public readonly T Start;
    public readonly T End;
    public readonly bool IsStartIncluded;
    public readonly bool IsEndIncluded;

    #endregion

    #region ctor

    public ValueTypeInterval(T start, T end, bool isStartIncluded, bool isEndIncluded)
    {
        if (EqualsViaComparable(start, end))
        {
            if (isStartIncluded ^ isEndIncluded)
            {
                throw new ArgumentException(
                    $"If '{nameof(start)}' equals '{nameof(end)}', both '{nameof(isStartIncluded)}' and '{nameof(isEndIncluded)}' must be either true or false.");

            }
        }

        var compare = start.CompareTo(end);
        if (compare > 0)
        {
            throw new ArgumentException(
                $"'{nameof(start)}' must be less than or equal to '{nameof(end)}'.",
                $"{nameof(start)}/{nameof(end)}");
        }

        this.Start = start;
        this.End = end;
        this.IsStartIncluded = isStartIncluded;
        this.IsEndIncluded = isEndIncluded;
    }

    #endregion

    #region Private

    private static bool EqualsViaComparable(T obj1, T obj2)
    {
        var equals = obj1.CompareTo(obj2) == 0;

        return equals;
    }

    private ValueTypeInterval<T> IntersectWithInternal(ValueTypeInterval<T> another, bool checkTrivialCases)
    {
        if (checkTrivialCases)
        {
            if (this.IsEmpty() || another.IsEmpty())
            {
                return CreateEmpty();
            }

            if (this.IsSubsetOf(another))
            {
                return this;
            }

            if (another.IsSubsetOf(this))
            {
                return another;
            }

            if (this.IsSingleValue() || another.IsSingleValue())
            {
                // not empty only if single-value set is subset of opposite set, but these two cases were before.

                return CreateEmpty();
            }
        }

        return FiniteIntervalIntersection(this, another);
    }

    private static ValueTypeInterval<T> FiniteIntervalIntersection(
        ValueTypeInterval<T> interval1,
        ValueTypeInterval<T> interval2)

    {
        var end1 = interval1.End;
        var start2 = interval2.Start!;

        var end1VsStart2 = end1.CompareTo(start2);

        if (end1VsStart2 < 0)
        {
            return CreateEmpty();
        }
        else if (end1VsStart2 == 0)
        {
            if (interval1.IsEndIncluded && interval2.IsStartIncluded)
            {
                return new ValueTypeInterval<T>(
                    interval1.End,
                    interval1.End,
                    true,
                    true);
            }

            return CreateEmpty();
        }
        else
        {
            var start1 = interval1.Start;
            var end2 = interval2.End;

            var start1VsEnd2 = start1.CompareTo(end2);

            if (start1VsEnd2 > 0)
            {
                return CreateEmpty();
            }
            else if (start1VsEnd2 == 0)
            {
                if (interval1.IsStartIncluded && interval2.IsEndIncluded)
                {
                    return new ValueTypeInterval<T>(
                        interval1.Start,
                        interval1.Start,
                        true,
                        true);
                }

                return CreateEmpty();
            }
            else
            {
                // two interval intersect
                T start;
                T end;

                bool isStartIncluded;
                bool isEndIncluded;

                var start1VsStart2 = start1.CompareTo(start2);
                if (start1VsStart2 < 0)
                {
                    start = start2;
                    isStartIncluded = interval2.IsStartIncluded;
                }
                else if (start1VsStart2 == 0)
                {
                    start = start2;
                    isStartIncluded = interval1.IsStartIncluded && interval2.IsStartIncluded;
                }
                else
                {
                    start = start1;
                    isStartIncluded = interval1.IsStartIncluded;
                }

                var end1VsEnd2 = end1.CompareTo(end2);
                if (end1VsEnd2 < 0)
                {
                    end = end1;
                    isEndIncluded = interval1.IsEndIncluded;
                }
                else if (end1VsEnd2 == 0)
                {
                    end = end1;
                    isEndIncluded = interval1.IsEndIncluded && interval2.IsEndIncluded;
                }
                else
                {
                    end = end2;
                    isEndIncluded = interval2.IsEndIncluded;
                }

                return new ValueTypeInterval<T>(
                    start,
                    end,
                    isStartIncluded,
                    isEndIncluded);
            }
        }
    }

    private bool IsSingleValue()
    {
        var isSingleValue =
            this.IsStartIncluded &&
            EqualsViaComparable(this.Start, this.End) &&
            true;

        return isSingleValue;
    }

    #endregion

    #region Public

    public bool IsSubsetOf(ValueTypeInterval<T> another)
    {
        if (this.IsEmpty())
        {
            return true; // empty set is subset of anything
        }

        if (another.IsEmpty())
        {
            return false; // nothing belongs to empty set
        }

        if (this.IsSingleValue(out var singleValue))
        {
            return another.Contains(singleValue);
        }

        // check start
        var compareStarts = this.Start.CompareTo(another.Start);

        if (compareStarts < 0)
        {
            return false;
        }
        else if (compareStarts == 0)
        {
            if (this.IsStartIncluded && !another.IsStartIncluded)
            {
                return false;
            }
        }

        // check end
        var compareEnds = this.End.CompareTo(another.End);
        if (compareEnds > 0)
        {
            return false;
        }
        else if (compareEnds == 0)
        {
            if (this.IsEndIncluded && !another.IsEndIncluded)
            {
                return false;
            }
        }

        return true;
    }

    public ValueTypeInterval<T> IntersectWith(ValueTypeInterval<T> another)
    {
        return this.IntersectWithInternal(another, true);
    }

    public bool Contains(T value)
    {
        if (this.IsEmpty())
        {
            return false;
        }

        var compareWithStart = this.Start.CompareTo(value);

        if (compareWithStart > 0)
        {
            return false;
        }
        else if (compareWithStart == 0 && !this.IsStartIncluded)
        {
            return false;
        }

        var compareWithEnd = this.End.CompareTo(value);
        if (compareWithEnd < 0)
        {
            return false;
        }
        else if (compareWithEnd == 0 && !this.IsEndIncluded)
        {
            return false;
        }

        return true;
    }

    public bool IsEmpty()
    {
        return
            !this.IsStartIncluded &&
            EqualsViaComparable(this.Start, this.End) &&
            true;
    }

    public bool IsSingleValue(out T value)
    {
        var isSingleValue =
            this.IsStartIncluded &&
            EqualsViaComparable(this.Start, this.End) &&
            true;

        value = isSingleValue ? this.Start : default!;

        return isSingleValue;
    }

    public static ValueTypeInterval<T> CreateEmpty()
    {
        var value = Activator.CreateInstance<T>();

        return new ValueTypeInterval<T>(
            value,
            value,
            false,
            false);
    }

    public static ValueTypeInterval<T> CreateSingleValue(T value)
    {
        return new ValueTypeInterval<T>(
            value,
            value,
            true,
            true);
    }

    #endregion

    #region IEquatable<ValueTypeInterval<T>> Members

    public bool Equals(ValueTypeInterval<T> other)
    {
        return
            EqualsViaComparable(this.Start, other.Start) &&
            EqualsViaComparable(this.End, other.End) &&
            IsStartIncluded == other.IsStartIncluded &&
            IsEndIncluded == other.IsEndIncluded;
    }

    #endregion

    #region Overridden

    public override bool Equals(object? obj)
    {
        return obj is ValueTypeInterval<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End, IsStartIncluded, IsEndIncluded);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.IsStartIncluded ? "[" : "(");

        sb.Append(this.Start);

        sb.Append(", ");

        sb.Append(this.End);

        sb.Append(this.IsEndIncluded ? "]" : ")");

        return sb.ToString();
    }

    #endregion
}

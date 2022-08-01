using System.Text;

namespace TauCode.Data;

public readonly struct Interval<T> : IEquatable<Interval<T>>
{
    #region Fields

    public readonly T Start;
    public readonly T End;
    public readonly bool IsStartIncluded;
    public readonly bool IsEndIncluded;

    #endregion

    #region ctor

    public Interval(T start, T end, bool isStartIncluded, bool isEndIncluded)
    {
        if (start == null)
        {
            if (isStartIncluded)
            {
                throw new ArgumentException("-∞ cannot be included.", $"{nameof(start)}/{nameof(isStartIncluded)}");
            }
        }

        if (end == null)
        {
            if (isEndIncluded)
            {
                throw new ArgumentException("+∞ cannot be included.", $"{nameof(end)}/{nameof(isEndIncluded)}");
            }
        }

        if (start != null && EqualsViaComparable(start, end))
        {
            if (isStartIncluded ^ isEndIncluded)
            {
                throw new ArgumentException(
                    $"If '{nameof(start)}' equals '{nameof(end)}' and their value is not null, both '{nameof(isStartIncluded)}' and '{nameof(isEndIncluded)}' must be either true or false.");

            }
        }

        if (start != null && end != null)
        {
            if (start is not IComparable startComparable)
            {
                throw new ArgumentException($"'{nameof(start)}' must implement IComparable.", nameof(start));
            }

            if (end is not IComparable endComparable)
            {
                throw new ArgumentException($"'{nameof(end)}' must implement IComparable.", nameof(end));
            }

            if (startComparable.GetType() != endComparable.GetType())
            {
                throw new ArgumentException($"Values of '{nameof(start)}' and '{nameof(end)}' must be of same type.");
            }

            var compare = startComparable.CompareTo(endComparable);
            if (compare > 0)
            {
                throw new ArgumentException(
                    $"'{nameof(start)}' must be less than or equal to '{nameof(end)}'.",
                    $"{nameof(start)}/{nameof(end)}");
            }
        }

        this.Start = start;
        this.End = end;
        this.IsStartIncluded = isStartIncluded;
        this.IsEndIncluded = isEndIncluded;
    }

    #endregion

    #region Public

    public bool IsSubsetOf(Interval<T> another)
    {
        if (this.Start == null && this.End == null)
        {
            // universal is subset only of universal
            return another.Start == null && another.End == null;
        }

        if (another.Start == null && another.End == null)
        {
            // universal is superset of anything
            return true;
        }

        if (this.IsInfinite() && another.IsFinite())
        {
            return false;
        }

        if (this.IsEmpty())
        {
            return true; // empty set is subset of anything
        }

        if (another.IsEmpty())
        {
            return false;
        }

        if (this.IsSingleValue(out var singleValue))
        {
            return another.Contains(singleValue);
        }

        #region finite intervals

        if (this.IsFinite() && another.IsFinite())
        {
            // intervals are finite => start and end cannot be null

            // check start
            var thisStartComparable = (IComparable)this.Start!;
            var anotherStartComparable = (IComparable)another.Start!;

            var compareStarts = thisStartComparable.CompareTo(anotherStartComparable);

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
            var thisEndComparable = (IComparable)this.End!;
            var anotherEndComparable = (IComparable)another.End!;

            var compareEnds = thisEndComparable.CompareTo(anotherEndComparable);
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

        #endregion

        #region belonging of (-∞, x}

        if (this.Start == null)
        {
            if (another.End == null)
            {
                return false;
            }

            // check end
            IComparable thisEndComparable = (IComparable)this.End!;
            IComparable anotherEndComparable = (IComparable)another.End;

            var compareEnds = thisEndComparable.CompareTo(anotherEndComparable);
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

        #endregion

        #region belonging of {x, +∞)

        if (this.End == null)
        {
            if (another.Start == null)
            {
                return false;
            }

            // check start
            IComparable thisStartComparable = (IComparable)this.Start;
            IComparable anotherStartComparable = (IComparable)another.Start;

            var compareStarts = thisStartComparable.CompareTo(anotherStartComparable);

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

            return true;
        }

        #endregion

        #region 'this' is finite: {x, y}

        if (another.Start == null)
        {
            var thisStartComparable = (IComparable)this.Start;
            var anotherEndComparable = (IComparable)another.End!;
            var thisStartCompare = thisStartComparable.CompareTo(anotherEndComparable); // sic. this_start vs another_end
            if (thisStartCompare >= 0)
            {
                return false;
            }

            var thisEndComparable = (IComparable)this.End;
            var thisEndCompare = thisEndComparable.CompareTo(anotherEndComparable);

            if (thisEndCompare > 0)
            {
                return false;
            }
            else if (thisEndCompare < 0)
            {
                return true;
            }
            else
            {
                // thisEndCompare == 0
                if (this.IsEndIncluded && !another.IsEndIncluded)
                {
                    return false;
                }
            }

            return true;
        }

        if (another.End == null)
        {
            var thisEndComparable = (IComparable)this.End;
            var anotherStartComparable = (IComparable)another.Start;

            var thisEndCompare = thisEndComparable.CompareTo(anotherStartComparable); // sic. compare this_end to another_start
            if (thisEndCompare <= 0)
            {
                return false;
            }

            var thisStartComparable = (IComparable)this.Start;
            var thisStartCompare = thisStartComparable.CompareTo(another.Start);

            if (thisStartCompare > 0)
            {
                return true;
            }
            else if (thisStartCompare < 0)
            {
                return false;
            }
            else
            {
                // thisStartCompare == 0
                if (this.IsStartIncluded && !another.IsStartIncluded)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        return false;
    }

    public Interval<T> IntersectWith(Interval<T> another)
    {
        return this.IntersectWithInternal(another, true);
    }

    public bool Contains(T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (this.IsUniversal())
        {
            return true;
        }

        if (this.IsEmpty())
        {
            return false;
        }

        var valueComparable = (IComparable)value;
        IComparable startComparable;
        IComparable endComparable;

        if (this.Start == null)
        {
            // start is -∞
            // care only about end
            endComparable = (IComparable)this.End!;
            var compare = valueComparable.CompareTo(endComparable);

            if (compare < 0)
            {
                return true;
            }
            else if (compare > 0)
            {
                return false;
            }
            else
            {
                // compare == 0
                return this.IsEndIncluded;
            }
        }

        if (this.End == null)
        {
            // end is +∞
            // care only about start
            startComparable = (IComparable)this.Start;
            var compare = valueComparable.CompareTo(startComparable);

            if (compare > 0)
            {
                return true;
            }
            else if (compare < 0)
            {
                return false;
            }
            else
            {
                // compare == 0
                return this.IsStartIncluded;
            }
        }

        startComparable = (IComparable)this.Start;
        endComparable = (IComparable)this.End;

        var compareWithStart = startComparable.CompareTo(valueComparable);

        if (compareWithStart > 0)
        {
            return false;
        }
        else if (compareWithStart == 0 && !this.IsStartIncluded)
        {
            return false;
        }

        var compareWithEnd = endComparable.CompareTo(valueComparable);
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
            this.Start != null &&
            this.End != null &&
            !this.IsStartIncluded &&
            EqualsViaComparable(this.Start, this.End) &&
            true;
    }

    public bool IsUniversal()
    {
        return this.Start == null && this.End == null;
    }

    public bool IsSingleValue(out T value)
    {
        var isSingleValue =
            this.Start != null &&
            this.End != null &&
            this.IsStartIncluded &&
            EqualsViaComparable(this.Start, this.End) &&
            true;

        value = isSingleValue ? this.Start : default!;

        return isSingleValue;
    }

    public bool IsInfinite() => this.Start == null || this.End == null;

    public bool IsFinite() => !this.IsInfinite();

    public static Interval<T> CreateEmpty()
    {
        var maybeNullableUnderlying = Nullable.GetUnderlyingType(typeof(T));
        if (maybeNullableUnderlying == null)
        {
            // T is not 'Nullable<SomeStruct>'
            var value = Activator.CreateInstance<T>();

            return new Interval<T>(
                value,
                value,
                false,
                false);
        }
        else
        {
            var singleValue = Activator.CreateInstance(maybeNullableUnderlying);

            return new Interval<T>(
                (T)singleValue! ?? throw new InvalidOperationException(),
                (T)singleValue,
                false,
                false);
        }
    }

    public static Interval<T> CreateSingleValue(T value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return new Interval<T>(
            value,
            value,
            true,
            true);
    }

    #endregion

    #region Private

    private static bool EqualsViaComparable(T obj1, T obj2)
    {
        if (obj1 == null)
        {
            return obj2 == null;
        }

        if (obj2 == null)
        {
            return false; // because obj1 != null
        }

        var obj1AsComparable = (IComparable)obj1;
        var equals = obj1AsComparable.CompareTo(obj2) == 0;

        return equals;
    }

    private Interval<T> IntersectWithInternal(Interval<T> another, bool checkTrivialCases)
    {
        if (checkTrivialCases)
        {
            if (this.IsEmpty() || another.IsEmpty())
            {
                return CreateEmpty();
            }

            if (this.IsUniversal())
            {
                return another;
            }

            if (another.IsUniversal())
            {
                return this;
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


        #region this is (-∞, x}

        if (this.Start == null)
        {
            // another.Start cannot be '-∞', otherwise one of intervals would have been belonging to another, but such a case was before
            // this.End cannot be +∞, otherwise 'this' would have been universal, but such a case was before

            var thisEndComparable = (IComparable)this.End!;
            var anotherStartComparable = (IComparable)another.Start!;

            var thisEndVsAnotherStart = thisEndComparable.CompareTo(anotherStartComparable);

            if (thisEndVsAnotherStart < 0)
            {
                return CreateEmpty();
            }
            else if (thisEndVsAnotherStart == 0)
            {
                if (this.IsEndIncluded && another.IsStartIncluded)
                {
                    return new Interval<T>(
                        this.End,
                        this.End,
                        true,
                        true);
                }

                return CreateEmpty();
            }
            else
            {
                // > 0
                return new Interval<T>(
                    another.Start,
                    this.End,
                    another.IsStartIncluded,
                    this.IsEndIncluded);
            }
        }

        #endregion

        #region this is {x, +∞)

        if (this.End == null)
        {
            // another.End cannot be '+∞', otherwise one of intervals would have been belonging to another, but such a case was before
            // this.Start cannot be '-∞', otherwise 'this' would have been universal, but such a case was before

            var thisStartComparable = (IComparable)this.Start;
            var anotherEndComparable = (IComparable)another.End!;

            var thisStartVsAnotherEnd = thisStartComparable.CompareTo(anotherEndComparable);

            if (thisStartVsAnotherEnd > 0)
            {
                return CreateEmpty();
            }
            else if (thisStartVsAnotherEnd == 0)
            {
                if (this.IsStartIncluded && another.IsEndIncluded)
                {
                    return new Interval<T>(
                        this.Start,
                        this.Start,
                        true,
                        true);
                }

                return CreateEmpty();
            }
            else
            {
                // < 0
                return new Interval<T>(
                    this.Start,
                    another.End,
                    this.IsStartIncluded,
                    another.IsEndIncluded);
            }
        }

        #endregion

        if (another.Start == null)
        {
            return another.IntersectWithInternal(this, false);
        }

        if (another.End == null)
        {
            return another.IntersectWithInternal(this, false);
        }

        return FiniteIntervalIntersection(this, another);
    }

    private static Interval<T> FiniteIntervalIntersection(Interval<T> interval1, Interval<T> interval2)
    {
        var end1 = (IComparable)interval1.End!;
        var start2 = (IComparable)interval2.Start!;

        var end1VsStart2 = end1.CompareTo(start2);

        if (end1VsStart2 < 0)
        {
            return CreateEmpty();
        }
        else if (end1VsStart2 == 0)
        {
            if (interval1.IsEndIncluded && interval2.IsStartIncluded)
            {
                return new Interval<T>(
                    interval1.End,
                    interval1.End,
                    true,
                    true);
            }

            return CreateEmpty();
        }
        else
        {
            var start1 = (IComparable)interval1.Start!;
            var end2 = (IComparable)interval2.End!;

            var start1VsEnd2 = start1.CompareTo(end2);

            if (start1VsEnd2 > 0)
            {
                return CreateEmpty();
            }
            else if (start1VsEnd2 == 0)
            {
                if (interval1.IsStartIncluded && interval2.IsEndIncluded)
                {
                    return new Interval<T>(
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
                    start = (T)start2;
                    isStartIncluded = interval2.IsStartIncluded;
                }
                else if (start1VsStart2 == 0)
                {
                    start = (T)start2;
                    isStartIncluded = interval1.IsStartIncluded && interval2.IsStartIncluded;
                }
                else
                {
                    start = (T)start1;
                    isStartIncluded = interval1.IsStartIncluded;
                }

                var end1VsEnd2 = end1.CompareTo(end2);
                if (end1VsEnd2 < 0)
                {
                    end = (T)end1;
                    isEndIncluded = interval1.IsEndIncluded;
                }
                else if (end1VsEnd2 == 0)
                {
                    end = (T)end1;
                    isEndIncluded = interval1.IsEndIncluded && interval2.IsEndIncluded;
                }
                else
                {
                    end = (T)end2;
                    isEndIncluded = interval2.IsEndIncluded;
                }

                return new Interval<T>(
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
            this.Start != null &&
            this.End != null &&
            this.IsStartIncluded &&
            EqualsViaComparable(this.Start, this.End) &&
            true;

        return isSingleValue;
    }

    #endregion

    #region IEquatable<Interval<T>> Members

    public bool Equals(Interval<T> other)
    {
        if (this.IsUniversal())
        {
            return other.IsUniversal();
        }

        if (this.IsEmpty())
        {
            return other.IsEmpty();
        }

        return
            EqualsViaComparable(this.Start, other.Start) &&
            EqualsViaComparable(this.End, other.End) &&
            this.IsStartIncluded == other.IsStartIncluded &&
            this.IsEndIncluded == other.IsEndIncluded &&
            true;
    }

    #endregion

    #region Overridden

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(this.IsStartIncluded ? "[" : "(");

        if (this.Start == null)
        {
            sb.Append("-∞");
        }
        else
        {
            sb.Append(this.Start);
        }

        sb.Append(", ");

        if (this.End == null)
        {
            sb.Append("+∞");
        }
        else
        {
            sb.Append(this.End);
        }

        sb.Append(this.IsEndIncluded ? "]" : ")");

        return sb.ToString();
    }

    public override bool Equals(object? obj)
    {
        return obj is Interval<T> other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        if (this.IsEmpty())
        {
            return 0;
        }

        if (this.IsUniversal())
        {
            return int.MaxValue;
        }

        return HashCode.Combine(
            this.Start,
            this.End,
            this.IsStartIncluded,
            this.IsEndIncluded);
    }

    #endregion
}
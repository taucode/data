using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace TauCode.Data
{
    public readonly struct Interval<T> // : IEquatable<Interval<T>> // todo
    {
        public readonly T Start;
        public readonly T End;
        public readonly bool IsStartIncluded;
        public readonly bool IsEndIncluded;

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

            if (start != null && Equals(start, end))
            {
                if (isStartIncluded ^ isEndIncluded)
                {
                    throw new ArgumentException(
                        $"If '{nameof(start)}' equals '{nameof(end)}' and their value is not null, both '{nameof(isStartIncluded)}' and '{nameof(isEndIncluded)}' must be either true or false.");

                }
            }

            if (start != null && end != null)
            {
                object startObj = start; // todo: looks excessive
                object endObj = end;

                var startComparable = startObj as IComparable;
                if (startComparable == null)
                {
                    throw new ArgumentException($"'{nameof(start)}' must implement IComparable.", nameof(start));
                }

                var endComparable = endObj as IComparable;
                if (endComparable == null)
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

        public bool IsSubsetOf(Interval<T> another)
        {
            if (this.Start == null && this.End == null)
            {
                // universum is subset only of universum
                return another.Start == null && another.End == null;
            }

            if (another.Start == null && another.End == null)
            {
                // universum is superset of anything
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

                // check end
                IComparable thisEndComparable = (IComparable)this.End;
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

            #region belonging of (-∞, x}

            if (this.Start == null)
            {
                if (another.End == null)
                {
                    return false;
                }

                // check end
                IComparable thisEndComparable = (IComparable)this.End;
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
                var anotherEndComparable = (IComparable)another.End;
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
            throw new NotImplementedException();
        }

        public bool Contains(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value)); // todo ut
            }

            if (this.Start == null && this.End == null)
            {
                return true;
            }

            var valueComparable = (IComparable)value;
            IComparable startComparable;
            IComparable endComparable;


            if (this.Start == null)
            {
                // start is -∞
                // care only about end
                endComparable = (IComparable)this.End;
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
                Equals(this.Start, this.End) &&
                true;
        }

        public bool IsSingleValue(out T value)
        {
            var isSingleValue =
                this.Start != null &&
                this.End != null &&
                this.IsStartIncluded &&
                Equals(this.Start, this.End) &&
                true;

            value = isSingleValue ? this.Start : default;

            return isSingleValue;
        }

        public bool IsInfinite() => this.Start == null || this.End == null;

        public bool IsFinite() => !this.IsInfinite();

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
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace TauCode.Data
{
    // todo: ut this thoroughly
    public readonly struct Interval<T> where T : IComparable<T>
    {
        public readonly T Start;
        public readonly T End;
        public readonly bool IsInclusive;

        public Interval(T start, T end, bool isInclusive)
        {
            if (start == null)
            {
                throw new NotImplementedException();
            }

            if (end == null)
            {
                throw new NotImplementedException();
            }

            this.Start = start;
            this.End = end;
            this.IsInclusive = isInclusive;
        }
    }
}

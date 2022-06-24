using System;
using System.Collections.Generic;
using System.Text;

namespace TauCode.Data
{
    public static class IntervalExtensions
    {
        public static bool IsSupersetOf<T>(this Interval<T> interval, Interval<T> another) =>
            another.IsSubsetOf(interval);
    }
}

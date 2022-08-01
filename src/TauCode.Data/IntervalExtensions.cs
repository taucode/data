namespace TauCode.Data
{
    public static class IntervalExtensions
    {
        public static bool IsSupersetOf<T>(this Interval<T> interval, Interval<T> another) =>
            another.IsSubsetOf(interval);
    }
}

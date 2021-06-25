namespace TauCode.Data
{
    internal static class Helper
    {
        internal static bool IsDecimalDigitInternal(this char c)
        {
            return c >= '0' && c <= '9';
        }
    }
}

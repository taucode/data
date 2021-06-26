using System;
using TauCode.Data.HostParsing;

namespace TauCode.Data
{
    internal static class Helper
    {
        internal static bool IsDecimalDigitInternal(this char c)
        {
            return c >= '0' && c <= '9';
        }

        internal static bool IsLatinLetterInternal(this char c)
        {
            if (c >= 'a' && c <= 'z')
            {
                return true;
            }

            if (c >= 'A' && c <= 'Z')
            {
                return true;
            }

            return false;
        }

        internal static bool IsUnicodeInternal(this char c) => c >= 256;

        internal static bool IsAllLatinLetters(this ReadOnlySpan<char> span)
        {
            foreach (var c in span)
            {
                if (!c.IsLatinLetterInternal())
                {
                    return false;
                }
            }

            return true;
        }

        internal static HostSegmentType? GetSpanSegmentType(this ReadOnlySpan<char> segmentSpan)
        {
            if (int.TryParse(segmentSpan, out var n))
            {
                if (n < 0)
                {
                    return null;
                }
                else if (n <= 255)
                {
                    return HostSegmentType.Numeric;
                }
                else
                {
                    return HostSegmentType.LongNumeric;
                }
            }
            else
            {
                if (segmentSpan.IsAllLatinLetters())
                {
                    return HostSegmentType.Ascii;
                }
                else
                {
                    return HostSegmentType.Unicode;
                }
            }
        }
    }
}

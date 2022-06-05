using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using TauCode.Data.Exceptions;

namespace TauCode.Data
{
    public readonly struct HostName : IEquatable<HostName>
    {
        #region Constants & Static

        private const int MaxLength = 253;
        private static readonly int MaxIPv6Length = "1111:2222:3333:4444:5555:6666:123.123.123.123".Length;

        private static readonly IdnMapping Idn; // IdnMapping type is thread-safe

        // todo: ut un-acceptable terminating chars
        internal static readonly HashSet<char> AcceptableTerminatingChars;

        static HostName()
        {
            Idn = new IdnMapping
            {
                UseStd3AsciiRules = true,
            };

            // todo: ut all. got into er-ror.
            var list = new List<char>
            {
                '\r',
                '\n',
                '\t',
                '\v',
                '\f',
                ' ',
                '~',
                '`',
                '!',
                '@',
                '#',
                '$',
                '%',
                '^',
                '&',
                '*',
                '(',
                ')',
                '=',
                '+',
                '\'',
                '"',
                '[',
                ']',
                '{',
                '}',
                '|',
                '/',
                '\\',
                ',',
                '?',
                '<',
                '>',
                ';',
            };

            AcceptableTerminatingChars = new HashSet<char>(list);
        }

        #endregion

        #region Fields

        public readonly HostNameKind Kind;
        public readonly string Value;

        #endregion

        #region ctor

        private HostName(HostNameKind kind, string value)
        {
            this.Kind = kind;
            this.Value = value;
        }

        #endregion

        #region Parsing

        public static HostName Parse(ReadOnlySpan<char> input)
        {
            var parsed = TryParse(input, out var hostName, out var exception);
            if (parsed)
            {
                return hostName;
            }

            throw exception;
        }

        public static bool TryParse(
            ReadOnlySpan<char> input,
            out HostName hostName,
            out TextDataExtractionException exception)
        {
            var consumed = TryExtract(
                input,
                out hostName,
                out exception,
                Helper.EmptyChars);

            return consumed.HasValue;
        }

        #endregion

        #region Extracting

        public static int? Extract(
            ReadOnlySpan<char> input,
            out HostName hostName,
            HashSet<char> terminatingChars = null)
        {
            var consumed = TryExtractInternal(
                input,
                out hostName,
                out var exception,
                terminatingChars);

            if (consumed.HasValue)
            {
                return consumed;
            }

            throw exception;
        }

        public static int? TryExtract(
            ReadOnlySpan<char> input,
            out HostName hostName,
            out TextDataExtractionException exception,
            HashSet<char> terminatingChars = null)
        {
            return TryExtractInternal(
                input,
                out hostName,
                out exception,
                terminatingChars);
        }

        #endregion

        #region Private

        public static int? TryExtractInternal(
            ReadOnlySpan<char> input,
            out HostName hostName,
            out TextDataExtractionException exception,
            HashSet<char> terminatingChars = null)
        {
            terminatingChars ??= AcceptableTerminatingChars;

            foreach (var c in terminatingChars)
            {
                if (!AcceptableTerminatingChars.Contains(c))
                {
                    throw new ArgumentException($"'{c}' is not a valid terminating char.", nameof(terminatingChars));
                }
            }

            if (input.IsEmpty)
            {
                hostName = default;
                exception = Helper.CreateException(ExtractionErrorTag.EmptyInput, null); // todo ut
                return null;
            }

            var canBeIPv6 = true;
            var periodCount = 0; // period is '.'
            var gotColon = false;
            var nothingButPeriodsAndDigits = true;
            var canBeAscii = true;
            var currentAsciiLabelLength = 0; // see Helper.MaxAsciiLabelLength to find out what label is.

            char? prevChar = null;
            var pos = 0;

            while (true)
            {
                if (
                    pos == input.Length ||
                    (pos == MaxLength && terminatingChars.Contains(input[pos]))
                    )
                {
                    if (
                        prevChar == '.' ||
                        prevChar == '-' ||
                        false
                    )
                    {
                        // these chars cannot be last ones
                        hostName = default;
                        exception = Helper.CreateException(ExtractionErrorTag.UnexpectedEnd, pos);
                        return null;
                    }

                    break;
                }

                if (pos == MaxLength)
                {
                    hostName = default;
                    exception = Helper.CreateException(ExtractionErrorTag.InputTooLong, pos);
                    return null;
                }

                if (canBeIPv6 && gotColon && pos == MaxIPv6Length)
                {
                    // got IPv6 here, cannot consume more chars

                    if (terminatingChars.Contains(input[pos]))
                    {
                        // got terminating char, let's try parse IPv6 below
                        break;
                    }
                    else
                    {
                        hostName = default;
                        exception = Helper.CreateException(ExtractionErrorTag.InvalidIPv6Address, 0);
                        return null;
                    }
                }

                var c = input[pos];

                if (terminatingChars.Contains(c)) // todo: ut this
                {
                    if (pos == 0)
                    {
                        hostName = default;
                        exception = Helper.CreateException(ExtractionErrorTag.EmptyInput, null); // note: not '0'
                        return null;
                    }

                    // got terminating char
                    if (
                        prevChar == '.' ||
                        prevChar == '-' ||
                        false
                    )
                    {
                        // these chars cannot be last ones
                        hostName = default;
                        exception = Helper.CreateException(ExtractionErrorTag.UnexpectedEnd, pos);
                        return null;
                    }

                    break;
                }

                if (c == '.')
                {
                    var badSituationForPeriod =
                        pos == 0 || // '.' cannot be first char
                        prevChar == '.' || // '.' cannot follow '.'
                        prevChar == '-' || // '.' cannot follow '-'
                        prevChar == ':' || // '.' cannot follow ':'
                        false;

                    if (badSituationForPeriod)
                    {
                        hostName = default;
                        exception = Helper.CreateException(ExtractionErrorTag.UnexpectedChar, pos);
                        return null;
                    }

                    periodCount++;

                    if (canBeAscii)
                    {
                        currentAsciiLabelLength =
                            0; // '.' cuts off previous label, if there was any. See Helper.MaxAsciiLabelLength to find out what label is.
                    }
                }
                else if (c.IsLatinLetterInternal())
                {
                    var isHexDigit = c.IsHexDigit();

                    nothingButPeriodsAndDigits = false;
                    if (gotColon)
                    {
                        if (isHexDigit)
                        {
                            // ok
                        }
                        else
                        {
                            // got colon, but now have a latin char that is not a hex digit => error.
                            hostName = default;
                            exception = Helper.CreateException(ExtractionErrorTag.UnexpectedChar, pos);
                            return null;
                        }
                    }

                    if (canBeAscii)
                    {
                        currentAsciiLabelLength++;
                    }

                    if (!isHexDigit)
                    {
                        canBeIPv6 = false;
                    }
                }
                else if (c == ':')
                {
                    if (!canBeIPv6)
                    {
                        exception = Helper.CreateException(ExtractionErrorTag.UnexpectedChar, pos);
                        hostName = default;
                        return null;
                    }

                    if (periodCount > 0)
                    {
                        // ':' cannot follow a '.'
                        exception = Helper.CreateException(ExtractionErrorTag.UnexpectedChar, pos);
                        hostName = default;
                        return null;
                    }

                    gotColon = true;
                    nothingButPeriodsAndDigits = false;

                    canBeAscii = false;
                    currentAsciiLabelLength = 0;
                }
                else if (c == '-')
                {
                    var badSituationForHyphen =
                        pos == 0 || // '-' cannot be first char
                        prevChar == '.' || // '-' cannot follow '.'
                        gotColon || // IPv6 address cannot hold '-'
                        false;

                    if (badSituationForHyphen)
                    {
                        hostName = default;
                        exception = Helper.CreateException(ExtractionErrorTag.UnexpectedChar, pos);
                        return null;
                    }

                    nothingButPeriodsAndDigits = false;

                    if (prevChar == '-')
                    {
                        canBeAscii = false; // two '-' in a row means internationalized domain name
                        currentAsciiLabelLength = 0;
                    }

                    if (canBeAscii)
                    {
                        currentAsciiLabelLength++;
                    }

                    canBeIPv6 = false;
                }
                else if (c.IsUnicodeInternal() || char.IsLetter(c))
                {
                    if (gotColon)
                    {
                        hostName = default;
                        exception = Helper.CreateException(ExtractionErrorTag.UnexpectedChar, pos);
                        return null;
                    }

                    canBeIPv6 = false;

                    canBeAscii = false;
                    currentAsciiLabelLength = 0;
                    nothingButPeriodsAndDigits = false;
                }
                else if (c.IsDecimalDigit())
                {
                    if (canBeAscii)
                    {
                        currentAsciiLabelLength++;
                    }
                }
                else
                {
                    // wrong char for a host name.
                    hostName = default;
                    exception = Helper.CreateException(ExtractionErrorTag.UnexpectedChar, pos);
                    return null;
                }

                if (currentAsciiLabelLength > Helper.MaxAsciiLabelLength)
                {
                    hostName = default;
                    exception = Helper.CreateException(ExtractionErrorTag.DomainLabelTooLong, pos);
                    return null;
                }

                prevChar = c;
                pos++;
            }

            if (gotColon)
            {
                if (!canBeIPv6) // todo resharper tells always false
                {
                    exception = Helper.CreateException(ExtractionErrorTag.InvalidIPv6Address, 0);
                    hostName = default;
                    return null;
                }

                // IPv6
                var parsed = IPAddress.TryParse(input[..pos], out var ipAddress);

                if (parsed)
                {
                    hostName = new HostName(HostNameKind.IPv6, ipAddress.ToString());
                    exception = null;
                    return pos;
                }
                else
                {
                    hostName = default;
                    exception = Helper.CreateException(ExtractionErrorTag.InvalidIPv6Address, 0);
                    return null;
                }
            }

            if (nothingButPeriodsAndDigits)
            {
                if (periodCount == 3)
                {
                    // might be IPv4
                    var parsed = IPAddress.TryParse(input[..pos], out var ipAddress);

                    if (parsed)
                    {
                        hostName = new HostName(HostNameKind.IPv4, ipAddress.ToString());
                        exception = null;
                        return pos;
                    }
                    else
                    {
                        exception = Helper.CreateException(ExtractionErrorTag.InvalidIPv4Address, 0);
                        hostName = default;
                        return null;
                    }
                }
                else
                {
                    // only numeric segments, but their count not equal to 4
                    hostName = default;
                    exception = Helper.CreateException(ExtractionErrorTag.InvalidHostName, 0);
                    return null;
                }
            }

            // ascii domain name
            if (canBeAscii)
            {
                hostName = new HostName(HostNameKind.Regular, input[..pos].ToString().ToLowerInvariant());
                exception = null;
                return pos;
            }

            // unicode domain name
            try
            {
                var ascii = Idn.GetAscii(input[..pos].ToString().ToLowerInvariant());
                hostName = new HostName(HostNameKind.Internationalized, ascii);
                exception = null;
                return pos;
            }
            catch
            {
                hostName = default;
                exception = Helper.CreateException(ExtractionErrorTag.InvalidHostName, 0);
                return null;
            }
        }

        #endregion

        #region IEquatable<HostName> Members

        public bool Equals(HostName other)
        {
            return
                this.Kind == other.Kind &&
                this.Value == other.Value;
        }

        #endregion

        #region Overridden

        public override bool Equals(object obj)
        {
            return obj is HostName other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)this.Kind, this.Value);
        }

        public override string ToString() => this.Value;

        #endregion
    }
}

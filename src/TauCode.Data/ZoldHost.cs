using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace TauCode.Data
{
    // todo get rid of.
    // todo regions, order
    public readonly struct ZoldHost : IEquatable<ZoldHost>
    {
        #region Nested

        private static class ErrorMessages
        {
            internal const string HostIsEmpty = "Host is empty.";
            internal const string UnexpectedCharacter = "Unexpected character.";
            internal const string InvalidHost = "Invalid host.";
            
        }

        private class ExtractionErrorInfoImpl : IExtractionErrorInfo
        {
            public int LineChange { get; internal set; }
            public int ColumnChange { get; internal set; }
            public int? IndexChange { get; internal set; }
            public char? Char { get; internal set; }
            public string Message { get; internal set; }
        }

        #endregion

        public static IExtractionErrorInfo LastHostExtractionErrorInfo
        {
            get
            {
                throw new NotImplementedException();
                //if (_wasError)
                //{
                //    return _lastHostExtractionErrorInfo;
                //}

                //return null;
            }
        }

        private const int MaxDomainNameLength = 253;
        private const int MaxSegmentCount = 127;
        private const int MaxIPv6AddressLength = 45; // "ffff:ffff:ffff:ffff:ffff:ffff:255.255.255.255".Length

        internal static readonly HashSet<char> AcceptableTerminatingChars;
        private static readonly HashSet<char> AcceptableIPv6Chars;
        private static readonly HashSet<char> AcceptableHostChars;

        private static readonly IdnMapping Idn; // IdnMapping type is thread-safe

        static ZoldHost()
        {
            var acceptableTerminatingChars = new[]
            {
                '\r',
                '\n',
                '\t',
                '\v',
                '\f',
                ' ',
            };

            AcceptableTerminatingChars = new HashSet<char>(acceptableTerminatingChars);

            Idn = new IdnMapping();
            Idn.UseStd3AsciiRules = true;

            // ipv6
            var ipv6List = new List<char>();
            ipv6List.AddCharRangeInternal('a', 'f');
            ipv6List.AddCharRangeInternal('A', 'F');
            ipv6List.AddCharRangeInternal('0', '9');
            ipv6List.Add(':');
            ipv6List.Add('.');

            AcceptableIPv6Chars = new HashSet<char>(ipv6List);
        }

        private ZoldHost(HostKind kind, string value)
        {
            this.Kind = kind;
            this.Value = value;
        }

        public ZoldHost(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var kind = DetectKind(value);
            if (kind == null)
            {
                throw new NotImplementedException();
            }

            this.Kind = kind.Value;
            this.Value = value;
        }

        private static HostKind? DetectKind(string value)
        {
            throw new NotImplementedException();
        }

        public HostKind Kind { get; }

        public string Value { get; }

        public bool Equals(ZoldHost other)
        {
            return Kind == other.Kind && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is ZoldHost other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) Kind, Value);
        }

        public static ZoldHost Parse(string s)
        {
            throw new NotImplementedException();
        }

        public static ZoldHost Parse(ReadOnlySpan<char> span)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse(string s, out ZoldHost host)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse(ReadOnlySpan<char> span, out ZoldHost? host)
        {
            throw new NotImplementedException();
        }


        // todo: public static TextLocationChange? TryExtract(ReadOnlySpan<char> span, out Host? host)

        public static TextLocationChange? TryExtract(string s, int start, out ZoldHost? host)
        {
            host = default;

            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            var stringLength = s.Length;

            if (start < 0 || start > stringLength)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            var remainingLength = stringLength - start;
            if (remainingLength == 0)
            {
                throw new NotImplementedException();
                //return Error(0, 0, 0, null, ErrorMessages.HostIsEmpty);
            }

            var index = start;

            var gotUnicode = false;
            var periodCount = 0;
            var gotAscii = false;
            var gotMinus = false;

            char? prevChar = null;

            int delta;
            char c;

            while (true)
            {
                if (index == stringLength)
                {
                    break;
                }

                delta = index - start;
                if (delta == MaxDomainNameLength)
                {
                    // last chance todo ut this
                    c = s[index];

                    if (AcceptableTerminatingChars.Contains(c))
                    {
                        break;
                    }

                    return null;
                }

                c = s[index];

                if (c == ':')
                {
                    if (gotUnicode || periodCount > 0)
                    {
                        return null;
                    }

                    return TryExtractIPv6(s, start, out host);
                }

                if (c.IsDecimalDigitInternal())
                {
                    index++;
                    prevChar = c;
                    continue;
                }

                if (c == '-')
                {
                    gotMinus = true;

                    if (prevChar == '-')
                    {
                        gotUnicode = true;
                    }

                    index++;
                    prevChar = c;
                    continue;
                }

                if (c.IsLatinLetterInternal())
                {
                    gotAscii = true;

                    index++;
                    prevChar = c;
                    continue;
                }

                if (c.IsUnicodeInternal())
                {
                    gotUnicode = true;

                    index++;
                    prevChar = c;
                    continue;
                }

                if (c == '.')
                {
                    if (prevChar == '.') // todo ut this
                    {
                        return null;
                    }

                    periodCount++;

                    index++;
                    prevChar = c;
                    continue;
                }

                if (AcceptableTerminatingChars.Contains(c))
                {
                    if (index == start)
                    {
                        // met terminator prior to any meaningful char.
                        throw new NotImplementedException();
                        //return Error(0, 0, 0, c, ErrorMessages.UnexpectedCharacter);
                    }

                    break;
                }

                return null; // unacceptable char.
            }

            delta = index - start;

            if (delta == 0)
            {
                throw new NotImplementedException();
                //return Error(0, 0, 0, null, ErrorMessages.HostIsEmpty);
            }

            if (!gotAscii && !gotUnicode && !gotMinus)
            {
                // only digits and periods around, should be an ip address.
                if (periodCount == 3)
                {
                    var parsed = IPAddress.TryParse(s.AsSpan(start, delta), out var address);
                    if (parsed)
                    {
                        host = new ZoldHost(HostKind.IPv4, address.ToString());
                        return new TextLocationChange(0, delta, null);
                    }
                }

                throw new NotImplementedException();
                //return Error(0, 0, 0, null, ErrorMessages.InvalidHost);
            }

            try
            {
                c = s[start + delta - 1];

                if (c == '.')
                {
                    return null;
                }

                var hostValue = Idn.GetAscii(s, start, delta).ToLowerInvariant();
                var hostKind = gotUnicode ? HostKind.InternationalizedDomainName : HostKind.RegularDomainName;
                host = new ZoldHost(hostKind, hostValue);
                return new TextLocationChange(0, delta, null);
            }
            catch (ArgumentException)
            {
                throw new NotImplementedException();
                //return Error(0, 0, 0, null, ErrorMessages.InvalidHost);
            }
        }

        private static TextLocationChange? TryExtractIPv6(string s, in int start, out ZoldHost? host)
        {
            var index = start;
            host = default;

            var stringLength = s.Length;
            int delta;

            while (true)
            {
                if (index == stringLength)
                {
                    break;
                }

                delta = index - start;

                char c;

                if (delta == MaxIPv6AddressLength)
                {
                    // last chance todo ut this
                    c = s[index];

                    if (AcceptableTerminatingChars.Contains(c))
                    {
                        break;
                    }

                    return null;

                }

                c = s[index];

                if (AcceptableIPv6Chars.Contains(c))
                {
                    index++;
                    continue;
                }

                if (AcceptableTerminatingChars.Contains(c))
                {
                    break;
                }

                return null;
            }

            delta = index - start;
            var span = s.AsSpan(start, delta);

            var parsed = IPAddress.TryParse(span, out var address);
            if (parsed)
            {
                host = new ZoldHost(HostKind.IPv6, address.ToString());
                return new TextLocationChange(0, delta, null);
            }

            return null;
        }

        // todo clean
        //private static TextLocationChange? Error(
        //    int lineChange,
        //    int columnChange,
        //    int? indexChange,
        //    char? @char,
        //    string message)
        //{
        //    _wasError = true;

        //    _lastHostExtractionErrorInfo.LineChange = lineChange;
        //    _lastHostExtractionErrorInfo.ColumnChange = columnChange;
        //    _lastHostExtractionErrorInfo.IndexChange = indexChange;
        //    _lastHostExtractionErrorInfo.Char = @char;
        //    _lastHostExtractionErrorInfo.Message = message;

        //    return null;
        //}
    }
}

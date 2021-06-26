using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;

namespace TauCode.Data
{
    // todo regions, order
    public readonly struct Host : IEquatable<Host>
    {
        private const int MaxDomainNameLength = 253;
        private const int MaxSegmentCount = 127;
        private const int MaxIPv6AddressLength = 45; // "ffff:ffff:ffff:ffff:ffff:ffff:255.255.255.255".Length

        private static readonly HashSet<char> AcceptableTerminatingChars;
        private static readonly HashSet<char> AcceptableIPv6Chars;
        private static readonly HashSet<char> AcceptableHostChars;

        private static readonly IdnMapping _idn; // IdnMapping type is thread-safe

        static Host()
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

            _idn = new IdnMapping();
            _idn.UseStd3AsciiRules = true;

            // ipv6
            var ipv6List = new List<char>();
            ipv6List.AddCharRangeInternal('a', 'f');
            ipv6List.AddCharRangeInternal('A', 'F');
            ipv6List.AddCharRangeInternal('0', '9');
            ipv6List.Add(':');
            ipv6List.Add('.');

            AcceptableIPv6Chars = new HashSet<char>(ipv6List);
        }

        private Host(HostKind kind, string value)
        {
            this.Kind = kind;
            this.Value = value;
        }

        public Host(string value)
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

        public bool Equals(Host other)
        {
            return Kind == other.Kind && Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is Host other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Kind, Value);
        }

        public static Host Parse(string s)
        {
            throw new NotImplementedException();
        }

        public static Host Parse(ReadOnlySpan<char> span)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse(string s, out Host host)
        {
            throw new NotImplementedException();
        }

        public static bool TryParse(ReadOnlySpan<char> span, out Host host)
        {
            throw new NotImplementedException();
        }

        public static TextLocation? TryExtract(string s, int start, out Host host)
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
                return null;
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
                    break;
                }

                return null; // unacceptable char.
            }

            delta = index - start;

            if (delta == 0)
            {
                return null;
            }

            if (!gotAscii && !gotUnicode && !gotMinus)
            {
                // only digits and periods around, should be an ip address.
                if (periodCount == 3)
                {
                    var parsed = IPAddress.TryParse(s.AsSpan(start, delta), out var address);
                    if (parsed)
                    {
                        host = new Host(HostKind.IPv4, address.ToString());
                        return new TextLocation(0, delta);
                    }
                }

                return null;
            }


            try
            {
                c = s[start + delta - 1];

                if (c == '.')
                {
                    return null;
                }

                var hostValue = _idn.GetAscii(s, start, delta).ToLowerInvariant();
                var hostKind = gotUnicode ? HostKind.InternationalizedDomainName : HostKind.RegularDomainName;
                host = new Host(hostKind, hostValue);
                return new TextLocation(0, delta);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static TextLocation? TryExtractIPv6(string s, in int startPosition, out Host host)
        {
            var index = startPosition;
            host = default;

            var stringLength = s.Length;
            int delta;

            while (true)
            {
                if (index == stringLength)
                {
                    break;
                }

                delta = index - startPosition;

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

            delta = index - startPosition;
            var span = s.AsSpan(startPosition, delta);

            var parsed = IPAddress.TryParse(span, out var address);
            if (parsed)
            {
                host = new Host(HostKind.IPv6, address.ToString());
                return new TextLocation(0, delta);
            }

            return null;
        }
    }
}

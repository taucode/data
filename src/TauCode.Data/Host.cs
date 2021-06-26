using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using TauCode.Data.HostParsing;

namespace TauCode.Data
{
    // todo regions, order
    public readonly struct Host : IEquatable<Host>
    {
        private const int MaxDomainNameLength = 253;
        private const int MaxSegmentCount = 127;
        private const int MaxIPv6AddressLength = 45; // "ffff:ffff:ffff:ffff:ffff:ffff:255.255.255.255".Length

        private static readonly HashSet<char> AcceptableTerminatingChars;
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

            var remainingLength = s.Length - start;
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

                var hostValue = _idn.GetAscii(s, start, delta);
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
            throw new NotImplementedException();

            //var index = 0;
            //host = default;

            //while (true)
            //{
            //    if (index == span.Length)
            //    {
            //        break;
            //    }

            //    var c = span[index];

            //    if (AcceptableTerminatingChars.Contains(c))
            //    {
            //        break;
            //    }

            //    if (index == MaxIPv6AddressLength)
            //    {
            //        return null;
            //    }

            //    index++;
            //}

            //var ipv6Span = span.Slice(0, index);
            //var parsed = IPAddress.TryParse(ipv6Span, out var address);

            //if (parsed)
            //{
            //    host = new Host(HostKind.IPv6, address.ToString());
            //    return new TextLocation(0, index);
            //}

            //return null;
        }

        public static TextLocation? TryExtractTodoRemove(ReadOnlySpan<char> span, out Host host)
        {
            host = default;

            if (span.Length == 0)
            {
                return null;
            }

            Span<HostSegment> hostSegments = stackalloc HostSegment[MaxSegmentCount];
            var segmentCount = 0;

            var gotUnicode = false;

            var index = 0;
            var segmentStart = 0;

            char? prevChar = null;

            while (true)
            {
                if (index == MaxDomainNameLength)
                {
                    throw new NotImplementedException();
                }

                if (index == span.Length)
                {
                    var delta = index - segmentStart;
                    var segmentSpan = span.Slice(segmentStart, delta);

                    var segmentType = segmentSpan.GetSpanSegmentType();
                    if (segmentType == null)
                    {
                        return null;
                    }

                    var start = (byte)segmentStart;
                    var length = (byte)(index - start);

                    if (length == 0)
                    {
                        return null;
                    }

                    var segment = new HostSegment(segmentType.Value, start, length);
                    hostSegments[segmentCount] = segment;
                    segmentCount++;

                    break;
                }

                var c = span[index];

                if (c == ':')
                {
                    throw new NotImplementedException();
                    //return TryExtractIPv6(span, out host);
                }

                if (c.IsDecimalDigitInternal())
                {
                    // ok, go on
                    prevChar = c;
                    index++;
                    continue;
                }

                if (c == '-')
                {
                    if (segmentStart == index)
                    {
                        return null;
                    }
                    else
                    {
                        // ok, go on
                        if (prevChar == '-')
                        {
                            gotUnicode = true;
                        }

                        prevChar = c;
                        index++;
                        continue;
                    }
                }

                if (c == '.')
                {
                    #region .

                    if (prevChar == '.') // todo: ut this case
                    {
                        // cannot have '..' in host
                        return null;
                    }

                    // got new segment

                    if (segmentCount == 0)
                    {
                        var segmentSpan = span.Slice(0, index);
                        if (segmentSpan.Length == 0)
                        {
                            return null;
                        }

                        var segmentType = segmentSpan.GetSpanSegmentType();
                        if (segmentType == null)
                        {
                            return null;
                        }

                        var segment = new HostSegment(segmentType.Value, 0, (byte)index);
                        hostSegments[segmentCount] = segment;
                        segmentCount++;

                        prevChar = c;
                        index++;
                        segmentStart = index;
                        continue;
                    }
                    else
                    {
                        var delta = index - segmentStart;
                        var segmentSpan = span.Slice(segmentStart, delta);

                        var segmentType = segmentSpan.GetSpanSegmentType();
                        if (segmentType == null)
                        {
                            return null;
                        }

                        var start = (byte)segmentStart;
                        var length = (byte)(index - start);

                        var segment = new HostSegment(segmentType.Value, start, length);
                        hostSegments[segmentCount] = segment;
                        segmentCount++;

                        prevChar = c;
                        index++;
                        segmentStart = index;
                        continue;
                    }

                    #endregion
                }

                if (c.IsLatinLetterInternal())
                {
                    prevChar = c;
                    index++;

                    continue;
                }
                else if (c.IsUnicodeInternal())
                {
                    gotUnicode = true;

                    prevChar = c;
                    index++;

                    continue;
                }

                if (AcceptableTerminatingChars.Contains(c))
                {
                    var delta = index - segmentStart;
                    var segmentSpan = span.Slice(segmentStart, delta);

                    var segmentType = segmentSpan.GetSpanSegmentType();
                    if (segmentType == null)
                    {
                        return null;
                    }

                    var start = (byte)segmentStart;
                    var length = (byte)(index - start);

                    var segment = new HostSegment(segmentType.Value, start, length);
                    hostSegments[segmentCount] = segment;
                    segmentCount++;

                    break;
                }

                return null;
            }

            if (index == 0)
            {
                return null;
            }

            if (segmentCount == 0)
            {
                // how could it happen?
                return null;
            }
            else if (segmentCount == 4)
            {
                // maybe got IPv4
                if (
                    hostSegments[0].Type == HostSegmentType.Numeric &&
                    hostSegments[1].Type == HostSegmentType.Numeric &&
                    hostSegments[2].Type == HostSegmentType.Numeric &&
                    hostSegments[3].Type == HostSegmentType.Numeric &&
                    true
                )
                {
                    var ipv4HostLength = hostSegments[3].Start + hostSegments[3].Length;
                    var ipv4Span = span.Slice(0, ipv4HostLength);
                    host = new Host(HostKind.IPv4, ipv4Span.ToString());
                    return new TextLocation(0, ipv4HostLength);
                }
            }

            var lastSegment = hostSegments[segmentCount - 1];
            if (lastSegment.Type == HostSegmentType.Numeric || lastSegment.Type == HostSegmentType.LongNumeric)
            {
                return null;
            }
            else if (gotUnicode)
            {
                var hostLength = hostSegments[segmentCount - 1].Start + hostSegments[segmentCount - 1].Length;
                var hostSpan = span.Slice(0, hostLength);

                var wannaBeHost = hostSpan.ToString();

                var idn = new IdnMapping();
                idn.UseStd3AsciiRules = true;

                try
                {
                    var asciiHost = idn.GetAscii(wannaBeHost);
                    host = new Host(HostKind.InternationalizedDomainName, asciiHost);
                    return new TextLocation(0, hostLength);
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                var hostLength = hostSegments[segmentCount - 1].Start + hostSegments[segmentCount - 1].Length;
                var hostSpan = span.Slice(0, hostLength);
                host = new Host(HostKind.RegularDomainName, hostSpan.ToString().ToLowerInvariant());
                return new TextLocation(0, hostLength);
            }
        }

        private static TextLocation? TryExtractIPv6TodoRemove(in ReadOnlySpan<char> span, out Host host)
        {
            var index = 0;
            host = default;

            while (true)
            {
                if (index == span.Length)
                {
                    break;
                }

                var c = span[index];

                if (AcceptableTerminatingChars.Contains(c))
                {
                    break;
                }

                if (index == MaxIPv6AddressLength)
                {
                    return null;
                }

                index++;
            }

            var ipv6Span = span.Slice(0, index);
            var parsed = IPAddress.TryParse(ipv6Span, out var address);

            if (parsed)
            {
                host = new Host(HostKind.IPv6, address.ToString());
                return new TextLocation(0, index);
            }

            return null;
        }
    }
}

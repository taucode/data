using System;
using System.Collections.Generic;

namespace TauCode.Data
{
    public readonly struct Host
    {
        internal static readonly HashSet<char> AcceptableTerminatingChars;

        public HostKind Kind { get; }

        public string Value { get; }

        public static TextLocationChange? TryExtract(ReadOnlySpan<char> span, out Host host)
        {
            throw new NotImplementedException();
        }
    }
}

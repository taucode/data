using System;

namespace TauCode.Data
{
    // todo regions, order
    public readonly struct Host : IEquatable<Host>
    {
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

        public static TextLocation? TryExtract(string s, int startPosition, out Host host)
        {
            throw new NotImplementedException();
        }

        public static TextLocation? TryExtract(ReadOnlySpan<char> span, out Host host)
        {
            throw new NotImplementedException();
        }
    }
}

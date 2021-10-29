using System;
using System.Collections.Generic;

namespace TauCode.Lab.Data
{
    public readonly struct Host
    {
        private static readonly HashSet<char> EmptyTerminationChars = new HashSet<char>();
        private static readonly HashSet<char> AcceptableTerminationChars;

        static Host()
        {
            var list = new List<char>
            {
                '\r',
                '\n',
                '\t',
                '\v',
                '\f',
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
                ':',
                ';',
            };

            AcceptableTerminationChars = new HashSet<char>(list);
        }

        public Host(HostKind kind, string value)
        {
            this.Kind = kind;
            this.Value = value;
        }

        public HostKind Kind { get; }

        public string Value { get; }

        public static TextLocationChange? TryExtract(
            ReadOnlySpan<char> span,
            out Host host,
            HashSet<char> terminationChars)
        {
            if (terminationChars == null)
            {
                terminationChars = EmptyTerminationChars;
            }
            else
            {
                if (!AcceptableTerminationChars.IsSupersetOf(terminationChars))
                {
                    throw new NotImplementedException(); // todo: unacceptable termination chars.
                }
            }

            throw new NotImplementedException();
        }
    }
}

using System;

namespace TauCode.Data.SemanticVersionSupport
{
    internal readonly struct SemanticVersionSegment : IComparable<SemanticVersionSegment>
    {
        internal SemanticVersionSegment(SemanticVersionSegmentType type, string value)
        {
            this.Type = type;
            this.Value = value;
        }

        internal SemanticVersionSegmentType Type { get; }

        internal string Value { get; }

        public int CompareTo(SemanticVersionSegment other)
        {
            if (this.Type == SemanticVersionSegmentType.Text && other.Type == SemanticVersionSegmentType.Text)
            {
                return string.CompareOrdinal(this.Value, other.Value);
            }

            if (this.Type == SemanticVersionSegmentType.Numeric && other.Type == SemanticVersionSegmentType.Numeric)
            {
                return CompareAsNumeric(this.Value, other.Value);
            }

            return this.Type.CompareTo(other.Type);
        }

        private int CompareAsNumeric(string s1, string s2)
        {
            var compareLengths = s1.Length.CompareTo(s2.Length);
            if (compareLengths != 0)
            {
                return compareLengths;
            }

            return string.CompareOrdinal(s1, s2);
        }
    }
}

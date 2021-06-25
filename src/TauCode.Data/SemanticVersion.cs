using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TauCode.Data
{
    public class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        #region Constants

        private const int MaxLength = 256;
        private const int MaxIntLength = 10;

        #endregion

        #region Private Static

        private static readonly HashSet<char> AcceptableSegmentChars;

        static SemanticVersion()
        {
            var chars = new List<char> { '-', '.' };

            for (var c = 'a'; c <= 'z'; c++)
            {
                chars.Add(c);
            }

            for (var c = 'A'; c <= 'Z'; c++)
            {
                chars.Add(c);
            }

            for (var c = '0'; c <= '9'; c++)
            {
                chars.Add(c);
            }

            AcceptableSegmentChars = new HashSet<char>(chars);

        }

        private static List<SemanticVersionSegment> BreakToSegments(string s, string paramName)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s)); // should never happen
            }

            var stringSegments = s.Split('.');

            var list = new List<SemanticVersionSegment>();

            foreach (var stringSegment in stringSegments)
            {
                var isNumeric = false;

                #region segment cannot be empty

                if (stringSegment.Length == 0)
                {
                    if (paramName == null)
                    {
                        return null;
                    }
                    else
                    {
                        throw new ArgumentException("Segment must not be empty.", paramName);
                    }
                }

                #endregion

                #region segment cannot be too long

                if (stringSegment.Length > MaxLength)
                {
                    if (paramName == null)
                    {
                        return null;
                    }
                    else
                    {
                        throw new ArgumentException("Segment is too long.", paramName);
                    }
                }

                #endregion

                #region check numeric segment

                if (stringSegment.All(x => x.IsDecimalDigitInternal()))
                {
                    if (stringSegment[0] == '0')
                    {
                        if (paramName == null)
                        {
                            return null;
                        }
                        else
                        {
                            throw new ArgumentException("Numeric segment cannot start with '0'.", paramName);
                        }
                    }

                    isNumeric = true;
                }

                #endregion

                #region check segment symbols are acceptable

                if (stringSegment.Any(x => !AcceptableSegmentChars.Contains(x)))
                {
                    if (stringSegment[0] == '0')
                    {
                        if (paramName == null)
                        {
                            return null;
                        }
                        else
                        {
                            throw new ArgumentException("Numeric segment cannot start with '0'.", paramName);
                        }
                    }
                }

                #endregion

                var type = isNumeric ? SemanticVersionSegmentType.Numeric : SemanticVersionSegmentType.Text;
                var segment = new SemanticVersionSegment(type, stringSegment);

                list.Add(segment);
            }

            return list;
        }


        #endregion

        #region Fields

        private readonly List<SemanticVersionSegment> _suffixSegments;

        #endregion

        #region .ctor

        public SemanticVersion(
            int major,
            int minor,
            int patch,
            string suffix,
            string metadata)
        {
            this.Major = major;
            this.Minor = minor;
            this.Patch = patch;

            _suffixSegments = null;
            if (suffix != null)
            {
                _suffixSegments = BreakToSegments(suffix, nameof(suffix));
            }
            this.Suffix = suffix;

            if (metadata != null)
            {
                BreakToSegments(metadata, nameof(metadata)); // just for checking
            }

            this.Metadata = metadata;
        }

        public SemanticVersion(string s)
        {
            var parsed = Parse(s);
            this.Major = parsed.Major;
            this.Minor = parsed.Minor;
            this.Patch = parsed.Patch;
            this.Suffix = parsed.Suffix;
            this.Metadata = parsed.Metadata;

            _suffixSegments = parsed._suffixSegments;
        }

        public SemanticVersion(ReadOnlySpan<char> span)
        {
            var parsed = Parse(span);
            this.Major = parsed.Major;
            this.Minor = parsed.Minor;
            this.Patch = parsed.Patch;
            this.Suffix = parsed.Suffix;
            this.Metadata = parsed.Metadata;

            _suffixSegments = parsed._suffixSegments;
        }


        #endregion

        #region Public

        public int Major { get; }

        public int Minor { get; }

        public int Patch { get; }

        public string Suffix { get; }

        public string Metadata { get; }

        public string ToString(bool includeMetadata)
        {
            var sb = new StringBuilder();

            sb.Append(this.Major);
            sb.Append('.');
            sb.Append(this.Minor);
            sb.Append('.');
            sb.Append(this.Patch);

            if (this.Suffix != null)
            {
                sb.Append('-');
                sb.Append(this.Suffix);
            }

            if (includeMetadata && this.Metadata != null)
            {
                sb.Append('+');
                sb.Append(this.Metadata);
            }

            var res = sb.ToString();
            return res;
        }

        public SemanticVersion GetReleaseVersion() => new SemanticVersion(
            this.Major,
            this.Minor,
            this.Patch,
            null,
            null);

        public string[] GetSuffixSegments()
        {
            return _suffixSegments
                .Select(x => x.Value)
                .ToArray();
        }

        #endregion

        #region Overridden

        public override string ToString() => this.ToString(true);

        public override bool Equals(object obj)
        {
            return obj is SemanticVersion other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Major, Minor, Patch, Suffix);
        }


        #endregion

        #region IEquatable<SemanticVersion> Members

        public bool Equals(SemanticVersion other)
        {
            return
                this.Major == other.Major &&
                this.Minor == other.Minor &&
                this.Patch == other.Patch &&
                this.Suffix == other.Suffix;
        }

        #endregion

        #region IComparable<SemanticVersion> Members

        public int CompareTo(SemanticVersion other)
        {
            var majorComparison = this.Major.CompareTo(other.Major);
            if (majorComparison != 0)
            {
                return majorComparison;
            }

            var minorComparison = this.Minor.CompareTo(other.Minor);
            if (minorComparison != 0)
            {
                return minorComparison;
            }

            var patchComparison = this.Patch.CompareTo(other.Patch);
            if (patchComparison != 0)
            {
                return patchComparison;
            }

            if (this.Suffix == null)
            {
                // 'this' is a release version
                if (other.Suffix == null)
                {
                    return 0;
                }

                return 1; // 'this' is a release, 'other' is pre-release => 'this' is bigger
            }

            if (other.Suffix == null)
            {
                // 'other' is a release version
                if (this.Suffix == null)
                {
                    return 0;
                }

                return -1; // 'this' is a pre-release, 'other' is release => 'this' is smaller
            }

            var minLength = Math.Min(this._suffixSegments.Count, other._suffixSegments.Count);

            for (var i = 0; i < minLength; i++)
            {
                var thisSegment = this._suffixSegments[i];
                var otherSegment = other._suffixSegments[i];

                var segmentComparison = thisSegment.CompareTo(otherSegment);
                if (segmentComparison != 0)
                {
                    return segmentComparison;
                }
            }


            return this._suffixSegments.Count.CompareTo(other._suffixSegments.Count);
        }

        #endregion

        #region Public Static

        public static SemanticVersion Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return Parse(s.AsSpan());
        }

        public static SemanticVersion Parse(ReadOnlySpan<char> span)
        {
            var parsed = TryParse(span, out var v);
            if (parsed)
            {
                return v;
            }

            throw new ArgumentException("Failed to parse semantic version.", nameof(span));
        }

        public static bool TryParse(string s, out SemanticVersion v)
        {
            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            return TryParse(s.AsSpan(), out v);
        }

        public static bool TryParse(ReadOnlySpan<char> span, out SemanticVersion v)
        {
            if (span.Length == 0 || span.Length > MaxLength)
            {
                v = default;
                return false;
            }

            var plusPosition = span.IndexOf('+');

            var versionWithoutMetadataSpan = span;
            var metadataSpan = ReadOnlySpan<char>.Empty;

            if (plusPosition >= 0)
            {
                // got metadata
                versionWithoutMetadataSpan = span.Slice(0, plusPosition);
                metadataSpan = span.Slice(plusPosition + 1);

                if (versionWithoutMetadataSpan.Length == 0 || metadataSpan.Length == 0)
                {
                    v = default;
                    return false;
                }
            }

            var releasePartSpan = versionWithoutMetadataSpan;
            var suffixSpan = ReadOnlySpan<char>.Empty;

            var minusPosition = span.IndexOf('-');
            if (minusPosition >= 0)
            {
                releasePartSpan = versionWithoutMetadataSpan.Slice(0, minusPosition);
                suffixSpan = versionWithoutMetadataSpan.Slice(minusPosition + 1);

                if (releasePartSpan.Length == 0 || suffixSpan.Length == 0)
                {
                    v = default;
                    return false;
                }
            }

            var releasePartParsed = ParseReleasePart(releasePartSpan, out var major, out var minor, out var patch);
            if (!releasePartParsed)
            {
                v = default;
                return false;
            }

            string suffix = null;
            if (suffixSpan.Length > 0)
            {
                suffix = suffixSpan.ToString();
            }

            string metadata = null;
            if (metadataSpan.Length > 0)
            {
                metadata = metadataSpan.ToString();
            }

            v = new SemanticVersion(major, minor, patch, suffix, metadata);
            return true;
        }

        private static bool ParseReleasePart(in ReadOnlySpan<char> releasePartSpan, out int major, out int minor, out int patch)
        {
            major = default;
            minor = default;
            patch = default;

            Span<int> numbers = stackalloc int[3];
            var numberCount = 0;

            var remainingSpan = releasePartSpan;

            while (true)
            {
                if (numberCount == 3)
                {
                    return false;
                }

                var dotPosition = remainingSpan.IndexOf('.');

                if (dotPosition >= 0)
                {
                    if (dotPosition == 0)
                    {
                        return false;
                    }

                    var numberSpan = remainingSpan.Slice(0, dotPosition);
                    if (numberSpan[0] == '0' && numberSpan.Length > 1)
                    {
                        return false;
                    }

                    var numberParsed = int.TryParse(numberSpan, out var n);
                    if (!numberParsed || n < 0)
                    {
                        return false;
                    }

                    numbers[numberCount] = n;
                    numberCount++;

                    remainingSpan = remainingSpan.Slice(dotPosition + 1);

                    continue;
                }
                else
                {
                    // all remaining span is ours.
                    if (remainingSpan.Length > MaxIntLength || remainingSpan.Length == 0)
                    {
                        return false;
                    }

                    if (remainingSpan[0] == '0' && remainingSpan.Length > 1)
                    {
                        return false;
                    }

                    var numberParsed = int.TryParse(remainingSpan, out var n);
                    if (!numberParsed || n < 0)
                    {
                        return false;
                    }

                    numbers[numberCount] = n;
                    numberCount++;
                    break;
                }
            }

            major = numbers[0];
            minor = numbers[1];
            patch = numbers[2];

            return true;
        }

        public static bool operator ==(SemanticVersion v1, SemanticVersion v2) => v1.Equals(v2);

        public static bool operator !=(SemanticVersion v1, SemanticVersion v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(SemanticVersion v1, SemanticVersion v2)
        {
            return v1.CompareTo(v2) < 0;
        }

        public static bool operator >(SemanticVersion v1, SemanticVersion v2)
        {
            return v1.CompareTo(v2) > 0;
        }

        public static bool operator <=(SemanticVersion v1, SemanticVersion v2)
        {
            return !(v1 > v2);
        }

        public static bool operator >=(SemanticVersion v1, SemanticVersion v2)
        {
            return !(v1 < v2);
        }
        
        #endregion
    }
}

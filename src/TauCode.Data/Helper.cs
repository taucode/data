using System.Collections.Generic;
using TauCode.Data.EmailAddressSupport;
using TauCode.Data.Exceptions;

namespace TauCode.Data
{
    internal static class Helper
    {
        internal static readonly HashSet<char> EmptyChars = new HashSet<char>();
        /// <summary>
        /// Label is a part of dot-separated ascii domain name, e.g.
        /// in case of domain name 'cor1.2.some-site.com',
        /// labels are: 'cor1', '2', 'some-site', 'com'.
        /// </summary>
        internal const int MaxAsciiLabelLength = 63;

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

        internal static bool IsHexDigit(this char c)
        {
            if (c >= 'a' && c <= 'f')
            {
                return true;
            }

            if (c >= 'A' && c <= 'F')
            {
                return true;
            }

            if (c >= '0' && c <= '9')
            {
                return true;
            }

            return false;
        }

        internal static bool IsDecimalDigit(this char c)
        {
            if (c >= '0' && c <= '9')
            {
                return true;
            }

            return false;
        }

        internal static bool IsUnicodeInternal(this char c) => c >= 256;

        internal static bool IsLocalPartSegment(this SegmentType segmentType)
        {
            return
                segmentType == SegmentType.LocalPartWord ||
                segmentType == SegmentType.Period ||
                segmentType == SegmentType.LocalPartQuotedString ||
                false;
        }

        internal static bool IsDomainSegment(this SegmentType segmentType)
        {
            return
                segmentType == SegmentType.Label ||
                segmentType == SegmentType.Period ||
                segmentType == SegmentType.IPAddress ||
                false;
        }

        // todo: ut all messages
        internal static string GetErrorMessage(ExtractionError error)
        {
            return error switch
            {
                // Common
                ExtractionError.EmptyInput => "Empty input.",
                ExtractionError.InputTooLong => "Input is too long.",
                ExtractionError.UnexpectedChar => "Unexpected character.",
                ExtractionError.UnexpectedEnd => "Unexpected end.",
                ExtractionError.InternalError => "Internal error.",

                // HostName
                ExtractionError.HostNameTooLong => "Host name is too long.",
                ExtractionError.DomainLabelTooLong => "Domain label is too long.",
                ExtractionError.InvalidHostName => "Invalid host name.",
                ExtractionError.InvalidIPv4Address => "Invalid IPv4 address.",
                ExtractionError.InvalidIPv6Address => "Invalid IPv6 address specification.",

                // EmailAddress
                ExtractionError.EmptyLocalPart => "Empty local part.",
                ExtractionError.LocalPartTooLong => "Local part is too long.",
                ExtractionError.EmailAddressTooLong => "Email address is too long.",
                ExtractionError.InvalidDomain => "Invalid domain.",
                ExtractionError.UnescapedSpecialCharacter => "Unescaped special character.",
                ExtractionError.UnclosedQuotedString => "Unclosed quoted string.",
                ExtractionError.EmptyQuotedString => "Empty quoted string.",
                ExtractionError.IPv4MustBeEnclosedInBrackets =>
                    "IPv4 address must be enclosed in '[' and ']'.",

                ExtractionError.NonEmojiChar => "Non-emoji character.",
                ExtractionError.IncompleteEmoji => "Incomplete emoji.",
                ExtractionError.BadEmoji => "Bad emoji.",

                _ => "Unknown error",
            };
        }

        internal static TextDataExtractionException CreateException(ExtractionError error, int? errorIndex)
        {
            // todo: ut all usages
            var message = Helper.GetErrorMessage(error);
            var ex = new TextDataExtractionException(message, errorIndex)
            {
                ExtractionError = error
            };
            return ex;
        }
    }
}

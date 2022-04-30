using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TauCode.Data.EmailAddressSupport;
using TauCode.Data.EmojiSupport;
using TauCode.Data.Exceptions;

namespace TauCode.Data
{
    public class EmailAddress : IEquatable<EmailAddress>
    {
        #region Constants & Read-only

        public const int MaxEmailAddressLength = 1000; // with all comments and folding whitespaces
        public const int MaxLocalPartLength = 64;
        public const int MaxCleanEmailAddressLength = 254;

        private static readonly HashSet<char> AcceptableTerminatingChars;
        private static readonly HashSet<char> EmptyTerminatingChars;
        private static readonly HashSet<char> AllowedSymbols;
        private static readonly HashSet<char> RightBracketTerminating;
        private static readonly char[] FoldingWhiteSpaceChars;

        #endregion

        #region Static

        static EmailAddress()
        {
            EmptyTerminatingChars = new HashSet<char>();
            FoldingWhiteSpaceChars = new[] { '\r', '\n', ' ' };

            AcceptableTerminatingChars = new HashSet<char>(HostName
                .AcceptableTerminatingChars
                .Except(new[]
                {
                    '(',
                    ')',
                    '[',
                    ']',
                }));

            AllowedSymbols = new HashSet<char>(new[]
            {
                '-',
                '+',
                '=',
                '!',
                '?',
                '%',
                '~',
                '$',
                '&',
                '/',
                '|',
                '{',
                '}',
                '#',
                '*',
                '^',
                '_',
                '`',
                '\''
            });

            RightBracketTerminating = new HashSet<char>(new[] { ']' });
        }

        #endregion

        #region Fields

        public readonly string LocalPart;
        public readonly HostName Domain;

        private string _value;
        private bool _valueBuilt;

        #endregion

        #region ctor

        private EmailAddress(string localPart, HostName hostName)
        {
            this.LocalPart = localPart;
            this.Domain = hostName;
        }

        #endregion

        #region Parsing

        public static EmailAddress Parse(ReadOnlySpan<char> input)
        {
            var consumed = TryExtractInternal(
                input,
                out var emailAddress,
                out var error,
                EmptyTerminatingChars);

            if (consumed == null)
            {
                throw error;
            }

            return emailAddress;
        }

        public static bool TryParse(
            ReadOnlySpan<char> input,
            out EmailAddress emailAddress,
            out TextDataExtractionException error)
        {
            var consumed = TryExtract(
                input,
                out emailAddress,
                out error,
                Helper.EmptyChars);

            return consumed.HasValue;
        }

        #endregion

        #region Extracting

        // todo: ut
        public static int? Extract(
            ReadOnlySpan<char> input,
            out EmailAddress emailAddress,
            HashSet<char> terminatingChars = null)
        {
            var consumed = TryExtractInternal(
                input,
                out emailAddress,
                out var error,
                terminatingChars);

            if (consumed == null)
            {
                throw error;
            }

            return consumed;
        }

        public static int? TryExtract(
            ReadOnlySpan<char> input,
            out EmailAddress emailAddress,
            out TextDataExtractionException error,
            HashSet<char> terminatingChars = null)
        {
            return TryExtractInternal(
                input,
                out emailAddress,
                out error,
                terminatingChars);
        }

        #endregion

        #region Private

        private static int? TryExtractInternal(
            ReadOnlySpan<char> input,
            out EmailAddress emailAddress,
            out TextDataExtractionException error,
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
                emailAddress = null;
                error = Helper.CreateException(ExtractionError.EmptyInput, null);
                return null;
            }

            var context = new EmailAddressExtractionContext(terminatingChars);

            #region extract local part

            while (true)
            {
                if (context.Index == MaxEmailAddressLength)
                {
                    emailAddress = null;
                    error = Helper.CreateException(ExtractionError.InputTooLong, context.Index);
                    return null;
                }

                var segment = TryExtractLocalPartSegment(input, context, out error);

                if (segment == null)
                {
                    emailAddress = null;
                    return null;
                }

                var segmentValue = segment.Value;
                var segmentValueType = segmentValue.Type;

                if (segmentValueType.IsLocalPartSegment())
                {
                    context.AddLocalPartSegment(segmentValue);
                }

                if (context.LocalPartLength > MaxLocalPartLength)
                {
                    // UT tag: 044523bc-6c75-4ef4-bac0-51ec9628fc0a
                    error = Helper.CreateException(ExtractionError.LocalPartTooLong, 0);
                    emailAddress = null;
                    return null;
                }

                if (segmentValueType == SegmentType.At)
                {
                    context.AtSymbolIndex = segmentValue.Start;
                    break;
                }
            }

            #endregion

            #region extract domain & pack result

            while (true)
            {
                // todo: check we are not out of MaxEmailAddressLength (and ut it!)
                if (context.Index == input.Length || context.IsAtTerminatingChar(input))
                {
                    if (context.DomainSegments.Count == 0)
                    {
                        emailAddress = null;
                        error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                        return null;
                    }

                    var lastDomainSegmentType = context.GetLastDomainSegmentType();
                    

                    // got to the end of the email, let's see what we're packing.
                    if (lastDomainSegmentType == SegmentType.Period)
                    {
                        var pos = context.GetDomainStartIndex();
                        error = Helper.CreateException(ExtractionError.InvalidDomain, pos);
                        emailAddress = null;
                        return null;
                    }

                    // local part
                    var localPart = BuildLocalPart(input, context);

                    // domain
                    if (context.DomainLength == 0)
                    {
                        error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                        emailAddress = null;
                        return null;
                    }

                    var domain = BuildDomain(input, context, out error);

                    if (domain == null)
                    {
                        var pos = context.GetDomainStartIndex();
                        // todo clean
                        //error = Helper.CreateException(ExtractionError.InvalidDomain, pos);
                        emailAddress = null;
                        return null;
                    }

                    if (domain.Value.Kind == HostNameKind.IPv4 && context.GetIPHostName() == null)
                    {
                        // looks like we've got something like joe@1.1.1.1

                        var pos = context.GetDomainStartIndex();
                        error = Helper.CreateException(ExtractionError.IPv4MustBeEnclosedInBrackets, pos);
                        emailAddress = null;
                        return null;
                    }

                    emailAddress = new EmailAddress(localPart, domain.Value);

                    if (emailAddress.ToString().Length > MaxCleanEmailAddressLength)
                    {
                        emailAddress = null;
                        error = Helper.CreateException(ExtractionError.EmailAddressTooLong, context.Index);
                        return null;
                    }

                    return context.Index;
                }

                if (context.Index == MaxEmailAddressLength)
                {
                    error = Helper.CreateException(ExtractionError.InputTooLong, context.Index);
                    emailAddress = null;
                    return null;
                }

                var segment = TryExtractDomainSegment(
                    input,
                    context,
                    out error);

                if (segment == null)
                {
                    emailAddress = null;
                    return null;
                }

                var segmentValue = segment.Value;

                var segmentValueType = segmentValue.Type;

                if (segmentValueType.IsDomainSegment())
                {
                    context.AddDomainSegment(segmentValue);
                }
            }

            #endregion
        }

        private static Segment? TryExtractLocalPartSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var c = input[context.Index];
            var lastLocalPartSegmentType = context.GetLastLocalPartSegmentType();

            if (
                char.IsLetterOrDigit(c) ||
                AllowedSymbols.Contains(c) ||
                c.IsEmojiStartingChar() ||
                false)
            {
                if (
                    lastLocalPartSegmentType == null ||
                    lastLocalPartSegmentType == SegmentType.Period ||
                    false)
                {
                    return TryExtractLocalPartWordSegment(input, context, out error);
                }

                error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                return null;
            }
            else if (c == '.')
            {
                if (
                    lastLocalPartSegmentType == SegmentType.LocalPartWord ||
                    lastLocalPartSegmentType == SegmentType.LocalPartQuotedString ||
                    false)
                {
                    var start = context.Index;
                    context.Index++;
                    error = null;
                    return new Segment(SegmentType.Period, start, 1, null);
                }
                else
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                    return null;
                }
            }
            else if (c == '@')
            {
                if (lastLocalPartSegmentType == null)
                {
                    error = Helper.CreateException(ExtractionError.EmptyLocalPart, context.Index);
                    return null;
                }

                if (
                    lastLocalPartSegmentType == SegmentType.LocalPartWord ||
                    lastLocalPartSegmentType == SegmentType.LocalPartQuotedString ||
                    false)
                {
                    var start = context.Index;
                    context.Index++;
                    error = null;
                    return new Segment(SegmentType.At, start, 1, null);
                }
                else
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                    return null;
                }
            }
            else if (c == ' ')
            {
                return TryExtractLocalPartSpaceSegment(input, context, out error);
            }
            else if (c == '\r')
            {
                return TryExtractLocalPartFoldingWhiteSpaceSegment(input, context, out error);
            }
            else if (c == '"')
            {
                return TryExtractLocalPartQuotedStringSegment(input, context, out error);
            }
            else if (c == '(')
            {
                return TryExtractCommentSegment(input, context, out error);
            }

            error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
            return null;
        }

        private static Segment? TryExtractDomainSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var c = input[context.Index];
            var lastNonCommentSegmentType = context.GetLastDomainSegmentType();

            if (char.IsLetterOrDigit(c))
            {
                if (context.GetIPHostName() != null)
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                    return null;
                }

                // we only want nothing or period before a label
                if (
                    lastNonCommentSegmentType == null ||
                    lastNonCommentSegmentType == SegmentType.Period ||
                    false)
                {
                    return TryExtractLabelSegment(input, context, out error);
                }
                else
                {
                    error = Helper.CreateException(ExtractionError.InvalidDomain, context.Index);
                    return null;
                }
            }
            else if (c == '.')
            {
                if (context.GetIPHostName() != null)
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                    return null;
                }

                // we only want label before period segment
                if (lastNonCommentSegmentType == SegmentType.Label)
                {
                    context.Index++;
                    error = null;
                    return new Segment(SegmentType.Period, (context.Index - 1), 1, null);
                }
                else
                {
                    error = Helper.CreateException(ExtractionError.InvalidDomain, context.Index);
                    return null;
                }
            }
            else if (c == '[')
            {
                if (context.GetIPHostName() != null || context.GotLabelOrPeriod())
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                    return null;
                }

                // we only want nothing 'clean' before ip address segment
                // todo: not really. comment can precede IP address. ut this.

                if (lastNonCommentSegmentType == null)
                {
                    if (context.Index < input.Length - 1)
                    {
                        var nextChar = input[context.Index + 1];
                        if (char.IsDigit(nextChar))
                        {
                            return TryExtractIPv4Segment(input, context, out error);
                        }

                        if (nextChar == 'I') // start of 'IPv6:' signature
                        {
                            return TryExtractIPv6Segment(input, context, out error);
                        }

                        context.Index++;
                        error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                        return null;
                    }
                    else
                    {
                        context.Index++;
                        error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                        return null;
                    }
                }

                error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                return null;

            }
            else if (c == '(')
            {
                return TryExtractCommentSegment(input, context, out error);
            }

            //error = EmailValidationError.UnexpectedCharacter; // todo: terminating char predicate here

            error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
            return null;
        }

        private static Segment? TryExtractCommentSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var start = context.Index;
            context.Index++; // input[start] is '('
            var length = input.Length;

            var depth = 1;

            var escapeMode = false;

            while (true)
            {
                if (context.Index == length)
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                    return null;
                }

                if (context.Index == MaxEmailAddressLength)
                {
                    error = Helper.CreateException(ExtractionError.InputTooLong, context.Index);
                    return null;
                }

                var c = input[context.Index];
                if (c == ')')
                {
                    context.Index++;

                    if (escapeMode)
                    {
                        escapeMode = false;
                    }
                    else
                    {
                        depth--;
                        if (depth == 0)
                        {
                            break;
                        }
                    }
                }
                else if (c == '(')
                {
                    context.Index++;

                    if (escapeMode)
                    {
                        escapeMode = false;
                    }
                    else
                    {
                        depth++;
                    }
                }
                else if (c == '\\')
                {
                    context.Index++;
                    escapeMode = !escapeMode;
                }
                else
                {
                    escapeMode = false;

                    var skipped = TrySkipEmoji(input[context.Index..], out var emojiError);
                    if (emojiError != null)
                    {
                        error = TransformInnerExtractionException(emojiError, context.Index);
                        return null;
                    }

                    if (skipped > 0)
                    {
                        context.Index += skipped;
                        continue;
                    }

                    if (
                        char.IsLetterOrDigit(c) ||
                        AllowedSymbols.Contains(c) ||
                        (
                            (
                                char.IsSymbol(c) ||
                                char.IsSeparator(c)
                            ) &&
                            c >= 0x80
                        ) ||
                        c == ' ' ||
                        c == '@' ||
                        c == '.' ||
                        c == ':' ||
                        c == '\r' || // todo this is wrong: mind folding whitespace
                        c == '\n' || // todo this is wrong: mind folding whitespace
                        c == '"' || // todo: unite into hashSet; todo: ut these chars
                        // todo: '[', ']' are accepted too.
                        false)
                    {
                        context.Index++;
                    }
                    else
                    {
                        // not an allowed char
                        error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                        return null;
                    }
                }
            }

            var delta = context.Index - start;
            error = null;
            return new Segment(SegmentType.Comment, start, delta, null);
        }

        private static int TrySkipEmoji(
            ReadOnlySpan<char> emojiSpan,
            out TextDataExtractionException error)
        {
            var skipped = EmojiHelper.Skip(emojiSpan, out var emojiExtractionError);
            var c = emojiSpan[0];

            switch (emojiExtractionError)
            {
                case null:
                    // successfully skipped emoji
                    error = null;
                    return skipped;

                case ExtractionError.NonEmojiChar:
                    switch (skipped)
                    {
                        case 0:
                            // 0th char was not emoji
                            // do nothing - following code will deal with it
                            error = null;
                            return 0;

                        case 1:
                            if (c.IsAsciiEmojiStartingChar())
                            {
                                error = null;
                                return 0;

                                // something like #, *, 0..9
                                // do nothing - following code will deal with it
                            }
                            else
                            {
                                error = Helper.CreateException(ExtractionError.BadEmoji, 0); // NOT 'skipped'
                                return 0;
                            }

                        default:
                            error = Helper.CreateException(ExtractionError.BadEmoji, 0); // NOT 'skipped'
                            return 0;
                    }

                case ExtractionError.IncompleteEmoji:
                    error = Helper.CreateException(ExtractionError.IncompleteEmoji, 0); // NOT 'skipped'
                    return skipped;

                default:
                    error = Helper.CreateException(ExtractionError.InternalError, null); // should never happen
                    return 0;
            }
        }

        #region Local Part Extraction

        private static Segment? TryExtractLocalPartSpaceSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var start = context.Index;
            context.Index++; // input[start] is a proper char since we've got here
            var length = input.Length;

            while (true)
            {
                if (context.Index - start > MaxLocalPartLength)
                {
                    error = Helper.CreateException(ExtractionError.LocalPartTooLong, context.Index);
                    return null;
                }

                if (context.Index == length)
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                    return null;
                }

                var c = input[context.Index];

                if (c == ' ')
                {
                    context.Index++;
                    continue;
                }

                // end of white space.
                break;
            }

            error = null;
            var delta = context.Index - start;
            return new Segment(SegmentType.LocalPartSpace, start, delta, null);
        }

        private static Segment? TryExtractLocalPartFoldingWhiteSpaceSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var start = context.Index;
            context.Index++; // input[start] is a proper char since we've got here
            var length = input.Length;

            var fwsLength = FoldingWhiteSpaceChars.Length;

            int delta;

            while (true)
            {
                if (context.Index == length)
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                    return null;
                }

                delta = context.Index - start;

                if (delta == fwsLength)
                {
                    break;
                }

                var c = input[context.Index];
                if (c == FoldingWhiteSpaceChars[delta])
                {
                    context.Index++;
                    continue;
                }

                error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                return null;
            }

            error = null;
            return new Segment(
                SegmentType.LocalPartFoldingWhiteSpace,
                start,
                delta,
                null); // actually, delta MUST be 3.
        }

        private static Segment? TryExtractLocalPartQuotedStringSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var start = context.Index;
            context.Index++; // skip '"'
            var length = input.Length;

            var escapeMode = false;

            while (true)
            {
                if (context.Index - start > MaxLocalPartLength)
                {
                    error = Helper.CreateException(ExtractionError.LocalPartTooLong, context.Index);
                    return null;
                }

                if (context.Index == length)
                {
                    error = Helper.CreateException(ExtractionError.UnclosedQuotedString, context.Index);
                    return null;
                }

                var c = input[context.Index];
                if (c == '"')
                {
                    context.Index++;

                    if (escapeMode)
                    {
                        // no more actions
                    }
                    else
                    {
                        break;
                    }

                    escapeMode = false;
                }
                else if (c == '\0' || c == '\r' || c == '\n')
                {
                    if (escapeMode)
                    {
                        context.Index++;
                    }
                    else
                    {
                        error = Helper.CreateException(ExtractionError.UnescapedSpecialCharacter, context.Index);
                        return null;
                    }

                    escapeMode = false;
                }
                
                else if (c == '\\')
                {
                    context.Index++;
                    escapeMode = !escapeMode;
                }
                else
                {
                    escapeMode = false;

                    var skipped = TrySkipEmoji(input[context.Index..], out var emojiError);
                    if (emojiError != null)
                    {
                        error = TransformInnerExtractionException(emojiError, context.Index);
                        return null;
                    }

                    if (skipped > 0)
                    {
                        context.Index += skipped;
                        continue;
                    }

                    if (
                        char.IsLetterOrDigit(c) ||
                        AllowedSymbols.Contains(c) ||
                        (
                            (
                                char.IsSymbol(c) ||
                                char.IsSeparator(c)
                            ) &&
                            c >= 0x80
                        ) ||
                        c == ' ' ||
                        c == '@' ||
                        c == '.' ||
                        c == ':' ||
                        c == '(' || // todo: unite into hashSet; todo: ut these chars
                        c == ')' || // todo: unite into hashSet; todo: ut these chars
                        c == ']' || // todo: unite into hashSet; todo: ut these chars
                        c == '[' || // todo: unite into hashSet; todo: ut these chars
                        false)
                    {
                        context.Index++;
                    }
                    else
                    {
                        error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                        return null;
                    }
                }
            }

            error = null;
            var delta = context.Index - start;

            if (delta == 2)
            {
                // todo: ut empty quoted string in the middle of local part
                // empty string
                error = Helper.CreateException(ExtractionError.EmptyQuotedString, context.Index - 2);
                context.Index = start;
                return null;
            }

            return new Segment(SegmentType.LocalPartQuotedString, start, delta, null);
        }

        private static Segment? TryExtractLocalPartWordSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var start = context.Index;

            var length = input.Length;

            while (true)
            {
                if (context.Index - start > MaxLocalPartLength)
                {
                    error = Helper.CreateException(ExtractionError.LocalPartTooLong, context.Index);
                    return null;
                }

                // todo: local part with comments too long, local part with comments is not too long when extract clean local part.

                if (context.Index == length)
                {
                    error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                    return null;
                }

                var c = input[context.Index];

                var skipped = TrySkipEmoji(input[context.Index..], out var emojiError);
                if (emojiError != null)
                {
                    error = TransformInnerExtractionException(emojiError, context.Index);
                    return null;
                }

                if (skipped > 0)
                {
                    context.Index += skipped;
                    continue;
                }

                if (
                    char.IsLetterOrDigit(c) ||
                    AllowedSymbols.Contains(c))
                {
                    // letter, digit or symbol => go on.
                    context.Index++;
                }
                else
                {
                    // not a char allowed in a word
                    break;
                }
            }

            error = null;
            var delta = context.Index - start;
            return new Segment(SegmentType.LocalPartWord, start, delta, null);
        }

        #endregion

        #region Domain Extraction

        private static Segment? TryExtractIPv6Segment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            // todo: ut john.doe@[IPv6:::] (success)
            var length = input.Length;
            var start = context.Index;
            context.Index++; // skip '['
            const string prefix = "IPv6:";
            const int prefixLength = 5; // "IPv6:".Length
            const int minRemainingLength =
                prefixLength +
                2 + /* :: */
                1; /* ] */

            var remaining = length - context.Index;

            if (remaining < minRemainingLength)
            {
                error = Helper.CreateException(ExtractionError.InvalidIPv6Address, context.Index);
                return null;
            }

            ReadOnlySpan<char> prefixSpan = prefix;
            if (input.Slice(context.Index, prefixLength).Equals(prefixSpan, StringComparison.Ordinal))
            {
                // good.
            }
            else
            {
                error = Helper.CreateException(ExtractionError.InvalidIPv6Address, context.Index);
                return null;
            }

            context.Index += prefixLength;

            var ipv6Span = input[context.Index..];
            var consumed = HostName.TryExtract(
                ipv6Span,
                out var hostName,
                out var hostNameError,
                RightBracketTerminating);

            if (consumed == null || hostName.Kind != HostNameKind.IPv6)
            {
                error = TransformInnerExtractionException(hostNameError, context.Index);
                return null;
            }

            context.Index += consumed.Value; // skip ipv6 address

            if (context.Index == length)
            {
                error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                return null;
            }

            var c = input[context.Index];

            if (c == ']')
            {
                context.Index++;

                var segmentLength = context.Index - start;
                var segment = new Segment(SegmentType.IPAddress, start, segmentLength, hostName);

                error = null;
                return segment;
            }

            // this should never happen, actually.
            error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
            return null;

        }

        private static Segment? TryExtractIPv4Segment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var start = context.Index;
            context.Index++; // skip '['

            var hostSpan = input[context.Index..];
            var consumed = HostName.TryExtract(
                hostSpan,
                out var hostName,
                out var hostNameError,
                RightBracketTerminating);

            if (consumed == null)
            {
                error = TransformInnerExtractionException(hostNameError, context.Index);
                return null;
            }

            // we gotta skip ']'
            context.Index += consumed.Value;

            if (context.Index == input.Length)
            {
                // we failed to achieve our ']'
                error = Helper.CreateException(ExtractionError.UnexpectedEnd, context.Index);
                return null;
            }

            var c = input[context.Index];

            if (c == ']')
            {
                context.Index++;
                error = null;

                var length =
                    1 + // '['
                    consumed.Value + // hostName
                    1 + // ']'
                    0;
                var segment = new Segment(SegmentType.IPAddress, start, length, hostName);
                return segment;
            }

            // this should never happen, actually.
            error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
            return null;
        }

        private static Segment? TryExtractLabelSegment(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var start = context.Index;
            var prevChar = input[start];
            context.Index++; // initial char is ok since we've got here
            var length = input.Length;

            while (true)
            {
                // todo: should we be aware of MaxEmailAddressLength? ut this.
                if (context.Index == length)
                {
                    break;
                }

                if (context.Index - start > Helper.MaxAsciiLabelLength)
                {
                    error = Helper.CreateException(ExtractionError.DomainLabelTooLong, start);
                    return null;
                }

                var c = input[context.Index];

                if (char.IsLetterOrDigit(c))
                {
                    prevChar = c;
                    context.Index++;
                    continue;
                }

                if (c == '-')
                {
                    if (prevChar == '.')
                    {
                        // '.' cannot be followed by '-'
                        error = Helper.CreateException(ExtractionError.InvalidDomain, context.Index);
                        return null;
                    }

                    prevChar = c;
                    context.Index++;
                    continue;
                }

                if (c == '(')
                {
                    // got start of comment
                    break;
                }

                if (c == '.')
                {
                    break;
                }

                if (context.TerminatingChars.Contains(c))
                {
                    break;
                }

                error = Helper.CreateException(ExtractionError.UnexpectedChar, context.Index);
                return null;
            }

            if (prevChar == '-')
            {
                // label cannot end with '-'
                error = Helper.CreateException(ExtractionError.InvalidDomain, context.Index);
                return null;
            }

            error = null;
            var delta = context.Index - start;
            return new Segment(SegmentType.Label, start, delta, null);
        }

        #endregion

        #region Building

        private static string BuildLocalPart(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context)
        {
            var localPartArray = new char[context.LocalPartLength];
            var pos = 0;

            foreach (var segment in context.LocalPartSegments)
            {
                var span = input.Slice(segment.Start, segment.Length);
                var destination = new Span<char>(localPartArray, pos, span.Length);
                span.CopyTo(destination);
                pos += span.Length;
            }

            var localPart = new string(localPartArray);
            return localPart;
        }

        private static HostName? BuildDomain(
            ReadOnlySpan<char> input,
            EmailAddressExtractionContext context,
            out TextDataExtractionException error)
        {
            var contextIPHostName = context.GetIPHostName();

            if (contextIPHostName != null)
            {
                error = null;
                return contextIPHostName;
            }

            // got 'domain name' host
            var domainArray = new char[context.DomainLength];
            var pos = 0;

            foreach (var segment in context.DomainSegments)
            {
                var span = input.Slice(segment.Start, segment.Length);
                var destination = new Span<char>(domainArray, pos, span.Length);
                span.CopyTo(destination);
                pos += span.Length;
            }

            var domainString = new string(domainArray);
            var parsed = HostName.TryParse(
                domainString,
                out var domain,
                out var hostNameError);

            if (hostNameError != null)
            {
                if (hostNameError.ExtractionError == ExtractionError.InputTooLong)
                {
                    error = Helper.CreateException(
                        ExtractionError.HostNameTooLong,
                        context.GetDomainStartIndex() + hostNameError.ErrorIndex);
                }
                else
                {
                    error = TransformInnerExtractionException(hostNameError, context.Index);
                }


                return null;
            }

            error = null;
            return domain;
        }

        private string BuildValue()
        {
            if (this.Domain.Value == null) // domain is default(HostName), which should not happen, actually
            {
                return null;
            }

            var sb = new StringBuilder();

            sb.Append(this.LocalPart);
            sb.Append("@");

            string format;

            switch (this.Domain.Kind)
            {
                case HostNameKind.Regular:
                case HostNameKind.Internationalized:
                    format = "{0}";
                    break;

                case HostNameKind.IPv4:
                    format = "[{0}]";
                    break;

                case HostNameKind.IPv6:
                    format = "[IPv6:{0}]";
                    break;

                default:
                    throw new FormatException("Cannot build email value.");
            }

            sb.AppendFormat(format, this.Domain.Value);
            var result = sb.ToString();
            return result;
        }

        #endregion

        private static TextDataExtractionException TransformInnerExtractionException(
            TextDataExtractionException error,
            int index)
        {
            return new TextDataExtractionException(error.Message, index + error.ErrorIndex ?? 0);
        }

        #endregion

        #region IEquatable<EmailAddress> Members

        public bool Equals(EmailAddress other)
        {
            if (other == null)
            {
                return false;
            }

            return
                this.LocalPart == other.LocalPart &&
                this.Domain.Equals(other.Domain);
        }

        #endregion

        #region Overridden

        public override bool Equals(object obj)
        {
            return obj is EmailAddress other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.LocalPart, this.Domain);
        }

        public override string ToString()
        {
            if (!_valueBuilt)
            {
                _value = this.BuildValue();
                _valueBuilt = true;
            }

            return _value;
        }

        #endregion
    }
}

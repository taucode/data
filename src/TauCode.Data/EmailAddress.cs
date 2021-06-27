using System;
using TauCode.Data.EmailAddressParsing;

namespace TauCode.Data
{
    public class EmailAddress
    {
        private const int MaxLocalPartLength = 64;
        private const int MaxEmailLength = 254;
        private const int MaxSubDomainLength = 63;
        private const int MaxLocalPartSegmentCount = MaxEmailLength / 2;

        public EmailAddress(string localPart, string domain)
        {
            // todo checks

            this.LocalPart = localPart;
            this.Domain = domain;
        }

        public string LocalPart { get; }
        public string Domain { get; }
        public Host CleanDomain { get; }
        public bool IsClean { get; set; }

        public EmailAddress ToCleanAddress()
        {
            throw new NotImplementedException();
        }

        public static TextLocation? TryExtract(string s, int start, out EmailAddress emailAddress)
        {
            emailAddress = default;

            if (s == null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (start < 0 || start > s.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            var span = s.AsSpan(start);
            var spanLength = span.Length;
            if (spanLength == 0)
            {
                return null;
            }

            // todo clean
            //var remainingLength = stringLength - start;
            //if (remainingLength == 0)
            //{
            //    return null;
            //}

            Span<EmailAddressSegment> segments = stackalloc EmailAddressSegment[MaxLocalPartSegmentCount];
            var segmentCount = 0;

            byte index = 0;
            TextLocationBuilder textLocationBuilder = default;

            EmailAddressSegmentType? lastNonCommentSegmentType = null;

            #region extract local part

            while (true)
            {
                if (index == spanLength)
                {
                    return null;
                }

                if (index == MaxEmailLength)
                {
                    //return new EmailValidationResult(EmailValidationError.EmailTooLong, index);

                    // todo: seems like wrong. what about last chance? (if current char is a whitespeace terminator) ut this!
                    return null; // todo clean
                }

                var segment = ExtractLocalPartSegment(
                    span,
                    lastNonCommentSegmentType,
                    ref textLocationBuilder,
                    ref index);

                if (segment == null)
                {
                    // todo clean
                    //return new EmailValidationResult(error, index);
                    return null;
                }

                var segmentValue = segment.Value;

                segments[segmentCount] = segmentValue;
                segmentCount++;

                if (
                    segmentValue.Type != EmailAddressSegmentType.Comment &&
                    segmentValue.Type != EmailAddressSegmentType.LocalPartSpace &&
                    segmentValue.Type != EmailAddressSegmentType.LocalPartFoldingWhiteSpace &&
                    true)
                {
                    lastNonCommentSegmentType = segmentValue.Type;
                }

                if (segmentValue.Type == EmailAddressSegmentType.At)
                {
                    break;
                }
            }

            #endregion

            var localPartPlusAtSegmentCount = segmentCount;
            lastNonCommentSegmentType = null;

            #region extract domain

            while (true)
            {
                if (index == spanLength)
                {
                    if (segmentCount == localPartPlusAtSegmentCount)
                    {
                        // todo clean
                        //return new EmailValidationResult(EmailValidationError.UnexpectedEnd, index);
                        return null;
                    }

                    // got to the end of the span, let's see what we're packing.
                    if (lastNonCommentSegmentType == EmailAddressSegmentType.Period)
                    {
                        // todo clean
                        //return new EmailValidationResult(EmailValidationError.InvalidDomainName, index);
                        return null;
                    }

                    //return new EmailValidationResult(EmailValidationError.NoError, null);
                    throw new NotImplementedException(); // we're good actually
                }

                if (index == MaxEmailLength)
                {
                    // todo clean
                    //return new EmailValidationResult(EmailValidationError.EmailTooLong, index);
                    return null;
                }

                var c = span[index];
                if (Host.AcceptableTerminatingChars.Contains(c))
                {
                    if (segmentCount == localPartPlusAtSegmentCount)
                    {
                        // no domain segments around
                        return null;
                    }

                    if (segments[segmentCount - 1].Type == EmailAddressSegmentType.Period)
                    {
                        // domain cannot end with period
                        return null;
                    }

                    break;
                }

                var segment = ExtractDomainSegment(
                    span,
                    lastNonCommentSegmentType,
                    ref textLocationBuilder,
                    ref index);

                if (segment == null)
                {
                    // todo clean
                    //return new EmailValidationResult(error, index);

                    return null;
                }

                var segmentValue = segment.Value;

                segments[segmentCount] = segmentValue;
                segmentCount++;

                if (segmentValue.Type != EmailAddressSegmentType.Comment)
                {
                    lastNonCommentSegmentType = segmentValue.Type;
                }
            }

            #endregion

            throw new NotImplementedException();
        }

        private static EmailAddressSegment? ExtractLocalPartSegment(
            in ReadOnlySpan<char> span,
            EmailAddressSegmentType? lastNonCommentSegmentType,
            ref TextLocationBuilder textLocationBuilder,
            ref byte index)
        {
            var c = span[index];

            if (
                char.IsLetterOrDigit(c) ||
                c == '_' ||
                //this.Settings.EffectiveAllowedSymbols.Contains(c) ||
                false
                )
            {
                if (
                    lastNonCommentSegmentType == null ||
                    lastNonCommentSegmentType == EmailAddressSegmentType.Period ||
                    false)
                {
                    return ExtractLocalPartWordSegment(
                        span,
                        ref textLocationBuilder,
                        ref index);
                }

                //error = EmailValidationError.UnexpectedCharacter;
                return null;
            }
            else if (c == '.')
            {
                throw new NotImplementedException();
                //if (
                //    lastNonCommentSegmentType == SegmentType.LocalPartWord ||
                //    lastNonCommentSegmentType == SegmentType.LocalPartQuotedString ||
                //    false)
                //{
                //    var start = index;
                //    index++;
                //    error = EmailValidationError.NoError;
                //    return new Segment(SegmentType.Period, start, 1);
                //}

                //error = EmailValidationError.UnexpectedCharacter;
                //return null;
            }
            else if (c == '@')
            {
                if (lastNonCommentSegmentType == null)
                {
                    // todo clean
                    //error = EmailValidationError.EmptyLocalPart;
                    return null;
                }

                if (
                    lastNonCommentSegmentType == EmailAddressSegmentType.LocalPartWord ||
                    lastNonCommentSegmentType == EmailAddressSegmentType.LocalPartQuotedString ||
                    false)
                {
                    var start = index;
                    index++;
                    textLocationBuilder.Column++;

                    // todo clean
                    //error = EmailValidationError.NoError;
                    return new EmailAddressSegment(EmailAddressSegmentType.At, start, 1);
                }

                // todo clean
                //error = EmailValidationError.UnexpectedCharacter;
                return null;
            }
            else if (c == ' ')
            {
                throw new NotImplementedException();
                //return this.ExtractLocalPartSpaceSegment(input, ref index, out error);
            }
            else if (c == '\r')
            {
                throw new NotImplementedException();
                //return this.ExtractLocalPartFoldingWhiteSpaceSegment(input, ref index, out error);
            }
            else if (c == '"')
            {
                throw new NotImplementedException();
                //return this.ExtractLocalPartQuotedStringSegment(input, ref index, out error);
            }
            else if (c == '(')
            {
                throw new NotImplementedException();
                //return this.ExtractCommentSegment(input, ref index, out error);
            }

            throw new NotImplementedException();

            //error = EmailValidationError.UnexpectedCharacter;
            //return null;
        }

        private static EmailAddressSegment? ExtractLocalPartWordSegment(
            in ReadOnlySpan<char> span,
            ref TextLocationBuilder textLocationBuilder,
            ref byte index)
        {
            var start = index;
            index++; // span[start] is a proper char since we've got here
            var length = span.Length;

            while (true)
            {
                if (index - start > MaxLocalPartLength)
                {
                    // todo clean
                    //error = EmailValidationError.LocalPartTooLong;
                    return null;
                }

                if (index == length)
                {
                    // todo clean
                    //error = EmailValidationError.UnexpectedEnd;
                    return null;
                }

                var c = span[index];

                if (
                    char.IsLetterOrDigit(c) ||
                    c == '_' ||
                    //this.Settings.EffectiveAllowedSymbols.Contains(c) ||
                    false
                    )
                {
                    index++;
                    continue;
                }
                // end of word.
                break;
            }

            // todo clean
            //error = EmailValidationError.NoError;
            var delta = index - start;
            textLocationBuilder.Column += delta;
            return new EmailAddressSegment(EmailAddressSegmentType.LocalPartWord, start, (byte)delta);
        }

        private static EmailAddressSegment? ExtractDomainSegment(
            in ReadOnlySpan<char> span,
            EmailAddressSegmentType? lastNonCommentSegmentType,
            ref TextLocationBuilder textLocationBuilder,
            ref byte index)
        {
            var c = span[index];

            if (char.IsLetterOrDigit(c))
            {
                // we only want nothing or period before sub-domain segment
                if (
                    lastNonCommentSegmentType == null ||
                    lastNonCommentSegmentType == EmailAddressSegmentType.Period ||
                    false)
                {
                    return ExtractSubDomainSegment(
                        span,
                        ref textLocationBuilder,
                        ref index);
                }

                // todo clean
                //error = EmailValidationError.InvalidDomainName;
                return null;
            }

            if (c == '.')
            {
                // we only want sub-domain before period segment
                throw new NotImplementedException();

                //if (lastNonCommentSegmentType == SegmentType.SubDomain)
                //{
                //    index++;
                //    error = EmailValidationError.NoError;
                //    return new Segment(SegmentType.Period, (byte)(index - 1), 1);
                //}

                //error = EmailValidationError.InvalidDomainName;
                //return null;
            }

            if (c == '[')
            {
                // we only want nothing before ip address segment
                throw new NotImplementedException();

                //if (lastNonCommentSegmentType == null)
                //{
                //    if (index < input.Length - 1)
                //    {
                //        var nextChar = input[index + 1];
                //        if (char.IsDigit(nextChar))
                //        {
                //            return this.ExtractIPv4Segment(input, ref index, out error);
                //        }

                //        if (nextChar == 'I') // start of 'IPv6:' signature
                //        {
                //            return this.ExtractIPv6Segment(input, ref index, out error);
                //        }

                //        index++;
                //        error = EmailValidationError.UnexpectedCharacter;
                //        return null;
                //    }

                //    index++;
                //    error = EmailValidationError.UnexpectedEnd;
                //    return null;
                //}

                //error = EmailValidationError.UnexpectedCharacter;
                //return null;

            }
            if (c == '(')
            {
                throw new NotImplementedException();
                //return this.ExtractCommentSegment(input, ref index, out error);
            }

            // todo clean
            //error = EmailValidationError.UnexpectedCharacter; // todo: terminating char predicate here
            return null;
        }

        private static EmailAddressSegment? ExtractSubDomainSegment(
            in ReadOnlySpan<char> span,
            ref TextLocationBuilder textLocationBuilder,
            ref byte index)
        {
            var start = index;
            var prevChar = span[start];
            index++; // initial char is ok since we've got here
            var length = span.Length;

            while (true)
            {
                if (index == length)
                {
                    break;
                }

                if (index - start > MaxSubDomainLength)
                {
                    // todo clean
                    //error = EmailValidationError.InvalidDomainName;
                    return null;
                }

                if (index == MaxEmailLength)
                {
                    // todo clean
                    //error = EmailValidationError.EmailTooLong;
                    return null;
                }

                var c = span[index];

                if (Host.AcceptableTerminatingChars.Contains(c))
                {
                    // got end of domain
                    if (prevChar == '-')
                    {
                        // domain name part cannot end with '-'
                        return null;
                    }

                    break;
                }

                if (char.IsLetterOrDigit(c))
                {
                    prevChar = c;
                    index++;
                    continue;
                }

                if (c == '-')
                {
                    if (prevChar == '.')
                    {
                        // '.' cannot be followed by '-'
                        // todo clean
                        //error = EmailValidationError.InvalidDomainName;
                        return null;
                    }

                    prevChar = c;
                    index++;
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

                // todo clean
                //error = EmailValidationError.InvalidDomainName;
                return null;
            }

            if (prevChar == '-')
            {
                // sub-domain cannot end with '-'

                //index--; // todo wtf?
                //error = EmailValidationError.InvalidDomainName;
                return null;
            }

            //error = EmailValidationError.NoError;
            var delta = index - start;
            textLocationBuilder.Column += delta;
            return new EmailAddressSegment(EmailAddressSegmentType.SubDomain, start, (byte)delta);
        }
    }
}

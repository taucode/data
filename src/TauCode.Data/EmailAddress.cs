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

            var length = s.Length;

            if (start < 0 || start > length)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            var remainingLength = length - start;
            if (remainingLength == 0)
            {
                return null;
            }

            Span<EmailAddressSegment> segments = stackalloc EmailAddressSegment[MaxLocalPartSegmentCount];
            var segmentCount = 0;

            var index = start;

            EmailAddressSegmentType? lastNonCommentSegmentType = null;

            

            throw new NotImplementedException();
        }
    }
}

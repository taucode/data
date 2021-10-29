namespace TauCode.Data.EmailAddressParsing
{
    internal readonly struct EmailAddressSegment
    {
        public EmailAddressSegment(EmailAddressSegmentType type, byte start, byte length)
        {
            this.Type = type;
            this.Start = start;
            this.Length = length;
        }

        public EmailAddressSegmentType Type { get; }
        public byte Start { get; }
        public byte Length { get; }
    }
}
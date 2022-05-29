namespace TauCode.Data.EmailAddressSupport
{
    public enum SegmentType : byte
    {
        Period = 1,
        Comment,

        LocalPartSpace,
        LocalPartFoldingWhiteSpace,
        LocalPartWord,
        LocalPartQuotedString,

        At, // '@' symbol

        Label, // part of sub-domain. e.g. in 'mail.google.com' labels are: 'mail', 'google', 'com'.
        IPAddress,
    }
}

namespace TauCode.Data.EmailAddressParsing
{
    public enum EmailAddressSegmentType : byte
    {
        Period = 1,
        Comment,

        LocalPartSpace,
        LocalPartFoldingWhiteSpace,
        LocalPartWord,
        LocalPartQuotedString,

        At, // '@' symbol

        SubDomain,
        IPAddress,
    }
}

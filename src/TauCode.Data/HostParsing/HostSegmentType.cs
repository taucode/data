namespace TauCode.Data.HostParsing
{
    internal enum HostSegmentType : byte
    {
        Numeric = 1,
        LongNumeric = 2, // more than 256
        Ascii = 3,
        Unicode = 4,
    }
}

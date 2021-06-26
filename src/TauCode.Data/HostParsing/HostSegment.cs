namespace TauCode.Data.HostParsing
{
    internal readonly struct HostSegment
    {
        private const int MaxSegmentLength = 64;

        internal HostSegment(HostSegmentType type, byte start, byte length)
        {
            this.Type = type;
            this.Start = start;
            this.Length = length;
        }

        internal HostSegmentType Type { get; }
        internal byte Start { get; }
        internal byte Length { get; }
    }
}
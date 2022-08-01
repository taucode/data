namespace TauCode.Data
{
    public readonly struct SpanDescriptor
    {
        public SpanDescriptor(int start, int length, bool startMustBeNonNegative = true)
        {
            if (start < 0 && startMustBeNonNegative)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            this.Start = start;
            this.Length = length;
        }

        public readonly int Start;
        public readonly int Length;
    }
}

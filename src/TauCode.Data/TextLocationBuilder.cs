namespace TauCode.Data
{
    internal struct TextLocationBuilder
    {
        internal int Line { get; set; }
        internal int Column { get; set; }

        internal TextLocation ToTextLocation() => new TextLocation(this.Line, this.Column);
    }
}

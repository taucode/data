namespace TauCode.Lab.Data
{
    public readonly struct TextLocationChange
    {
        public TextLocationChange(int indexChange, int lineChange, int columnChange)
        {
            this.IndexChange = indexChange;
            this.LineChange = lineChange;
            this.ColumnChange = columnChange;
        }

        public int IndexChange { get; }

        public int LineChange { get; }

        public int ColumnChange { get; }
    }
}

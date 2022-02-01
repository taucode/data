using System;

namespace TauCode.Data
{
    public readonly struct TextLocationChange : IEquatable<TextLocationChange>
    {
        public static readonly TextLocationChange Zero = new TextLocationChange(0, 0, 0);

        public TextLocationChange(int lineChange, int columnChange, int? indexChange)
        {
            this.LineChange = lineChange;
            this.ColumnChange = columnChange;
            this.IndexChange = indexChange;
        }

        public int LineChange { get; }

        public int ColumnChange { get; }

        public int? IndexChange { get; }

        public bool Equals(TextLocationChange other)
        {
            return
                this.LineChange == other.LineChange &&
                this.ColumnChange == other.ColumnChange;
        }

        public override bool Equals(object obj)
        {
            return
                obj is TextLocationChange other &&
                this.Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.LineChange, this.ColumnChange);
        }

        public static bool operator ==(TextLocationChange change1, TextLocationChange change2) =>
            change1.Equals(change2);

        public static bool operator !=(TextLocationChange change1, TextLocationChange change2) => !(change1 == change2);
    }
}

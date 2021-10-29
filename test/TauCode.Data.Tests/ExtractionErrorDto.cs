namespace TauCode.Data.Tests
{
    public class ExtractionErrorDto
    {
        public int LineChange { get; set; }
        public int ColumnChange { get; set; }
        public int? IndexChange { get; set; }
        public char? Char { get; set; }
        public string Message { get; set; }
    }
}

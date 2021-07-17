namespace TauCode.Data
{
    public interface IExtractionErrorInfo
    {
        int LineChange { get; }
        int ColumnChange { get; }
        int? IndexChange { get; }
        char? Char { get; }
        string Message { get; }
    }
}

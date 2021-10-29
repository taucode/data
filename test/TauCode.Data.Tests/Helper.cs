namespace TauCode.Data.Tests
{
    internal static class Helper
    {
        internal static ExtractionErrorDto ToDto(this IExtractionErrorInfo errorInfo)
        {
            return new ExtractionErrorDto
            {
                LineChange = errorInfo.LineChange,
                ColumnChange = errorInfo.ColumnChange,
                IndexChange = errorInfo.IndexChange,
                Char = errorInfo.Char,
                Message = errorInfo.Message,
            };
        }

        internal static TextLocationChangeDto ToDto(this TextLocationChange textLocationChange)
        {
            return new TextLocationChangeDto
            {
                LineChange = textLocationChange.LineChange,
                ColumnChange = textLocationChange.ColumnChange,
                IndexChange = textLocationChange.IndexChange,
            };
        }

        internal static HostDto ToDto(this Host host)
        {
            return new HostDto
            {
                Kind = host.Kind,
                Value = host.Value,
            };
        }
    }
}

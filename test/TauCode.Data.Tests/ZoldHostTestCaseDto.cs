namespace TauCode.Data.Tests
{
    // todo get rid of class and of json.
    public class ZoldHostTestCaseDto
    {
        public string TestName { get; set; }
        public string Host { get; set; }
        public string ExpectedHost { get; set; }
        public HostKind ExpectedHostKind { get; set; }
        public TextLocationChangeDto ExpectedTextLocationChange { get; set; }
        public ExtractionErrorDto ExpectedError { get; set; }
        
        public string Comment { get; set; }

        public override string ToString() => this.Host;
    }
}

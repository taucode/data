namespace TauCode.Data.Tests
{
    public class HostTestCaseDto
    {
        public string TestName { get; set; }
        public string Host { get; set; }
        public string ExpectedHost { get; set; }
        public HostKind ExpectedHostKind { get; set; }
        public int? ExpectedColumnShift { get; set; }
        public string Comment { get; set; }

        public override string ToString() => this.Host;
    }
}

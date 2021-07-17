namespace TauCode.Data.Tests
{
    public class HostTestCaseDto
    {
        public string TestName { get; set; }
        public string Host { get; set; }
        public HostDto ExpectedHost { get; set; }
        public TextLocationChangeDto ExpectedTextLocationChange { get; set; }
        public ExtractionErrorDto ExpectedError { get; set; }
        public string Comment { get; set; }

        public override string ToString()
        {
            var result = $"{this.Host} ({this.TestName})";

            // todo temp
            if (this.Host == "")
            {
                result = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!" + result;
            }

            return result;
        }
    }
}

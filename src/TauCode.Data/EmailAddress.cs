namespace TauCode.Data
{
    public class EmailAddress
    {
        public EmailAddress(string localPart, Host domain)
        {
            // todo checks

            this.LocalPart = localPart;
            this.Domain = domain;
        }

        public string LocalPart { get; }
        public Host Domain { get; }
    }
}

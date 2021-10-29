namespace TauCode.Data
{
    public enum HostKind : byte
    {
        Unknown = 0,

        RegularDomainName = 1,
        InternationalizedDomainName = 2,
        IPv4 = 3,
        IPv6 = 4,
    }
}

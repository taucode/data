using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TauCode.Data.Tests
{
    public class HostDto
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public HostKind Kind { get; set; }

        public string Value { get; set; }
    }
}

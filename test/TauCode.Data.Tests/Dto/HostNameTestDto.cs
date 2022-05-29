using System.Text;

namespace TauCode.Data.Tests.Dto;

public class HostNameTestDto
{
    public int? Index { get; set; }
    public string TestHostName { get; set; }
    public int? ExpectedResult { get; set; }
    public string ExpectedHostName { get; set; }
    public HostNameKind? ExpectedHostNameKind { get; set; }
    public string Comment { get; set; }
    public ErrorDto ExpectedError { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        if (this.Index.HasValue)
        {
            sb.Append($"{this.Index:0000} ");
        }

        sb.Append($"'{this.TestHostName}'");
        return sb.ToString();
    }
}

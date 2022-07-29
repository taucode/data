namespace TauCode.Data.Tests.Dto.TestDto;

public class CtorTestDto
{
    public int Index { get; set; }

    public string? TestInterval { get; set; }
    public IntervalDto? ExpectedInterval { get; set; }
    public ExceptionDto? ExceptionException { get; set; }

    public override string ToString() => Index.ToString("0000") + " " + TestInterval;
}

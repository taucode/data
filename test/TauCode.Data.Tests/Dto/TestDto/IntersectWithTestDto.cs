namespace TauCode.Data.Tests.Dto.TestDto;

public class IntersectWithTestDto
{
    public int? TextLineNumber { get; set; } // starting with '1', as in the text editor.

    public IntervalDto TestInterval1 { get; set; } = null!;
    public IntervalDto TestInterval2 { get; set; } = null!;
    public IntervalDto ExpectedResult { get; set; } = null!;

    public override string ToString() => $"{TextLineNumber!.Value:0000} {TestInterval1} ∩ {TestInterval2} = {ExpectedResult}";
}

namespace TauCode.Data.Tests.Dto.TestDto;

public class IsSubsetOfTestDto
{
    public int? TextLineNumber { get; set; }

    public IntervalDto TestInterval1 { get; set; } = null!;
    public IntervalDto TestInterval2 { get; set; } = null!;
    public bool ExpectedResult { get; set; }

    public override string ToString()
    {
        var symbol = ExpectedResult ? '⊆' : '⊄';
        return $"{TextLineNumber!.Value:0000} {TestInterval1} {symbol} {TestInterval2}";
    }
}

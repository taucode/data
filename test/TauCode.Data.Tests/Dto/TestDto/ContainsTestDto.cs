namespace TauCode.Data.Tests.Dto.TestDto;

public class ContainsTestDto
{
    public int? TextLineNumber { get; set; } // starting with '1', as in the text editor.

    public int TestValue { get; set; }
    public IntervalDto? TestInterval { get; set; }
    public bool ExpectedResult { get; set; }

    public override string ToString()
    {
        var symbol = ExpectedResult ? '⊆' : '⊄';
        return $"{TextLineNumber!.Value:0000} {TestValue} {symbol} {TestInterval}";
    }
}

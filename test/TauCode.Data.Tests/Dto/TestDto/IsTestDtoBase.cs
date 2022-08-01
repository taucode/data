namespace TauCode.Data.Tests.Dto.TestDto;

public abstract class IsTestDtoBase
{
    public int? TextLineNumber { get; set; } // starting with '1', as in the text editor.

    public IntervalDto TestInterval { get; set; } = null!;
    public bool ExpectedResult { get; set; }

    public override string ToString()
    {
        return $"{TextLineNumber!.Value:0000} {this.ExpectedResult.ToString().ToLowerInvariant().PadLeft(6)} : {this.TestInterval}";
    }
}

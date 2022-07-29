using System.Text;

namespace TauCode.Data.Tests.Dto;

public class IntervalDto
{
    public IntervalDto()
    {
    }

    public IntervalDto(int? start, int? end, bool isStartIncluded, bool isEndIncluded)
    {
        this.Start = start;
        this.End = end;
        this.IsStartIncluded = isStartIncluded;
        this.IsEndIncluded = isEndIncluded;
    }

    public IntervalDto(string s)
    {
        ParseIntervalString(s, out var start, out var end, out var isStartIncluded, out var isEndIncluded);

        this.Start = start;
        this.End = end;
        this.IsStartIncluded = isStartIncluded;
        this.IsEndIncluded = isEndIncluded;
    }

    private static void ParseIntervalString(
        string s,
        out int? start,
        out int? end,
        out bool isStartIncluded,
        out bool isEndIncluded)
    {
        if (s.Trim() == "∅")
        {
            start = 0;
            end = 0;

            isStartIncluded = false;
            isEndIncluded = false;

            return;
        }

        var parts = s
            .Split(',')
            .Select(x => x.Trim())
            .ToList();

        // start
        var startDelimiter = parts.First()[0];
        if (startDelimiter == '[')
        {
            isStartIncluded = true;
        }
        else if (startDelimiter == '(')
        {
            isStartIncluded = false;
        }
        else
        {
            throw new Exception();
        }

        var startString = parts.First()[1..].Trim();
        if (startString == "-∞")
        {
            start = null;
        }
        else
        {
            start = int.Parse(startString);
        }

        // end
        var endDelimiter = parts.Skip(1).Single()[^1];
        if (endDelimiter == ']')
        {
            isEndIncluded = true;
        }
        else if (endDelimiter == ')')
        {
            isEndIncluded = false;
        }
        else
        {
            throw new Exception();
        }

        var endString = parts.Skip(1).Single()[..^1].Trim();
        if (endString == "+∞")
        {
            end = null;
        }
        else
        {
            end = int.Parse(endString);
        }
    }

    public int? Start { get; set; }
    public int? End { get; set; }
    public bool IsStartIncluded { get; set; }
    public bool IsEndIncluded { get; set; }

    public bool IsEmpty()
    {
        return
            this.Start.HasValue &&
            this.End.HasValue &&
            this.Start.Value == this.End.Value &&
            !this.IsStartIncluded &&
            !this.IsEndIncluded &&
            true;
    }

    public override string? ToString()
    {
        if (IsEmpty())
        {
            return "∅";
        }

        var sb = new StringBuilder();
        sb.Append(this.IsStartIncluded ? "[" : "(");

        if (this.Start == null)
        {
            sb.Append("-∞");
        }
        else
        {
            sb.Append(this.Start);
        }

        sb.Append(", ");

        if (this.End == null)
        {
            sb.Append("+∞");
        }
        else
        {
            sb.Append(this.End);
        }

        sb.Append(this.IsEndIncluded ? "]" : ")");

        return sb.ToString();
    }
}

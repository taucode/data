using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TauCode.Data.Tests.Dto;

public class IntervalCtorTestDto
{
    public class IntervalDto
    {
        public int? Start { get; set; }
        public int? End { get; set; }
        public bool IsStartIncluded { get; set; }
        public bool IsEndIncluded { get; set; }

        public override string ToString()
        {
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

    public int? Index { get; set; }

    public string TestInterval { get; set; }
    public IntervalDto ExpectedInterval { get; set; }
    public ExceptionDto ExceptionException { get; set; }

    public override string ToString() => this.Index.Value.ToString("0000") + " " + this.TestInterval;
}

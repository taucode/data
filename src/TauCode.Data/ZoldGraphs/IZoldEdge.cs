using System.Collections.Generic;

// todo: get rid of Zoo, here & anywhere.
namespace TauCode.Data.ZoldGraphs
{
    public interface IZoldEdge
    {
        IZoldNode From { get; }
        IZoldNode To { get; }
        IDictionary<string, object> Properties { get; set; }
        void Disappear();
    }
}

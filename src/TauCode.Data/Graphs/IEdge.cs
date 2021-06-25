using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public interface IEdge
    {
        INode From { get; }
        INode To { get; }
        IDictionary<string, object> Properties { get; set; }
        void Disappear();
    }
}

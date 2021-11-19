using System.Collections.Generic;

namespace TauCode.Data.ZoldGraphs
{
    public interface IZoldNode
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        IZoldEdge DrawEdgeTo(IZoldNode another);

        IReadOnlyCollection<IZoldEdge> OutgoingEdges { get; }

        IReadOnlyCollection<IZoldEdge> IncomingEdges { get; }
    }
}

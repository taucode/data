using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public interface INode
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        IEdge DrawEdgeTo(INode another);

        IReadOnlyCollection<IEdge> OutgoingEdges { get; }

        IReadOnlyCollection<IEdge> IncomingEdges { get; }
    }
}

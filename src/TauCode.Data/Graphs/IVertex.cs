using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public interface IVertex
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        IReadOnlyCollection<IEdge> OutgoingEdges { get; }

        IReadOnlyCollection<IEdge> IncomingEdges { get; }
    }
}

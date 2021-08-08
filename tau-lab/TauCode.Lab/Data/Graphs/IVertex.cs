using System.Collections.Generic;

namespace TauCode.Lab.Data.Graphs
{
    public interface IVertex
    {
        string Name { get; set; }

        IReadOnlyCollection<IEdge> OutgoingEdges { get; }

        IReadOnlyCollection<IEdge> IncomingEdges { get; }
    }
}

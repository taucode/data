using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public interface IGraph
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        void AddNode(INode node);

        bool ContainsNode(INode node);

        bool RemoveNode(INode node);

        IReadOnlyCollection<INode> Nodes { get; }

        IEnumerable<IEdge> Edges { get; }
    }
}

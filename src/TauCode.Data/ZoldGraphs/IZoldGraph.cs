using System.Collections.Generic;

namespace TauCode.Data.ZoldGraphs
{
    public interface IZoldGraph
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        void AddNode(IZoldNode node);

        bool ContainsNode(IZoldNode node);

        bool RemoveNode(IZoldNode node);

        IReadOnlyCollection<IZoldNode> Nodes { get; }

        IEnumerable<IZoldEdge> Edges { get; }
    }
}

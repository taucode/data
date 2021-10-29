using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Lab.Data.Graphs
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Vertex : IVertex
    {
        #region Fields

        private readonly EdgeCollection _outgoingEdges;
        private readonly EdgeCollection _incomingEdges;

        #endregion

        #region ctor

        public Vertex()
        {
            _outgoingEdges = new EdgeCollection(this);
            _incomingEdges = new EdgeCollection(this);
        }

        public Vertex(string name)
            : this()
        {
            this.Name = name;
        }

        #endregion

        #region Internal

        internal void AddOutgoingEdge(Edge edge)
        {
            _outgoingEdges.AddEdge(edge);
        }

        internal void AddIncomingEdge(Edge edge)
        {
            _incomingEdges.AddEdge(edge);
        }

        #endregion

        #region IVertex Members

        public string Name { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public IReadOnlyCollection<IEdge> OutgoingEdges => _outgoingEdges;

        public IReadOnlyCollection<IEdge> IncomingEdges => _incomingEdges;

        #endregion
    }
}

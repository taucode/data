using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Data.Graphs
{
    // todo :get rid of 'rho'
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Node : INode
    {
        #region Fields

        private readonly HashSet<Edge> _outgoingEdges;
        private readonly HashSet<Edge> _incomingEdges;

        #endregion

        #region Constructor

        public Node()
        {
            _outgoingEdges = new HashSet<Edge>();
            _incomingEdges = new HashSet<Edge>();
        }

        #endregion

        #region IRhoNode Members

        public string Name { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public IEdge DrawEdgeTo(INode another)
        {
            if (another == null)
            {
                throw new ArgumentNullException(nameof(another));
            }

            var castedAnother = another as Node;
            if (castedAnother == null)
            {
                throw new ArgumentException($"Expected node of type '{typeof(Node).FullName}'.", nameof(another));
            }

            var edge = new Edge(this, castedAnother);

            this._outgoingEdges.Add(edge);
            castedAnother._incomingEdges.Add(edge);

            return edge;
        }

        public IReadOnlyCollection<IEdge> OutgoingEdges => _outgoingEdges;

        public IReadOnlyCollection<IEdge> IncomingEdges => _incomingEdges;

        #endregion

        #region Internal

        internal void RemoveOutgoingEdge(Edge outgoingEdge)
        {
            _outgoingEdges.Remove(outgoingEdge);
        }

        internal void RemoveIncomingEdge(Edge incomingEdge)
        {
            _incomingEdges.Remove(incomingEdge);
        }

        #endregion
    }
}

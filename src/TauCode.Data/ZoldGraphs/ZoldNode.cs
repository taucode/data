using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Data.ZoldGraphs
{
    // todo :get rid of 'rho'
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class ZoldNode : IZoldNode
    {
        #region Fields

        private readonly HashSet<ZoldEdge> _outgoingEdges;
        private readonly HashSet<ZoldEdge> _incomingEdges;

        #endregion

        #region Constructor

        public ZoldNode()
        {
            _outgoingEdges = new HashSet<ZoldEdge>();
            _incomingEdges = new HashSet<ZoldEdge>();
        }

        #endregion

        #region IRhoNode Members

        public string Name { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public IZoldEdge DrawEdgeTo(IZoldNode another)
        {
            if (another == null)
            {
                throw new ArgumentNullException(nameof(another));
            }

            var castedAnother = another as ZoldNode;
            if (castedAnother == null)
            {
                throw new ArgumentException($"Expected node of type '{typeof(ZoldNode).FullName}'.", nameof(another));
            }

            var edge = new ZoldEdge(this, castedAnother);

            this._outgoingEdges.Add(edge);
            castedAnother._incomingEdges.Add(edge);

            return edge;
        }

        public IReadOnlyCollection<IZoldEdge> OutgoingEdges => _outgoingEdges;

        public IReadOnlyCollection<IZoldEdge> IncomingEdges => _incomingEdges;

        #endregion

        #region Internal

        internal void RemoveOutgoingEdge(ZoldEdge outgoingEdge)
        {
            _outgoingEdges.Remove(outgoingEdge);
        }

        internal void RemoveIncomingEdge(ZoldEdge incomingEdge)
        {
            _incomingEdges.Remove(incomingEdge);
        }

        #endregion
    }
}

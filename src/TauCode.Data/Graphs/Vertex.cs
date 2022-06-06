using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Data.Graphs
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Vertex : IVertex
    {
        #region Fields

        private readonly ArcCollection _outgoingArcs;
        private readonly ArcCollection _incomingArcs;

        #endregion

        #region ctor

        public Vertex()
        {
            _outgoingArcs = new ArcCollection(this);
            _incomingArcs = new ArcCollection(this);
        }

        public Vertex(string name)
            : this()
        {
            this.Name = name;
        }

        #endregion

        #region Internal

        internal void AddOutgoingArc(Arc arc)
        {
            _outgoingArcs.AddArc(arc);
        }

        internal void AddIncomingArc(Arc arc)
        {
            _incomingArcs.AddArc(arc);
        }

        #endregion

        #region IVertex Members

        public string Name { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public IReadOnlyCollection<IArc> OutgoingArcs => _outgoingArcs;

        public IReadOnlyCollection<IArc> IncomingArcs => _incomingArcs;

        #endregion
    }
}

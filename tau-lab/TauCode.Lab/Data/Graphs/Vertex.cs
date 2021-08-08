using System.Collections.Generic;

namespace TauCode.Lab.Data.Graphs
{
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

        #region IVertex Members

        public string Name { get; set; }

        public IReadOnlyCollection<IEdge> OutgoingEdges => _outgoingEdges;

        public IReadOnlyCollection<IEdge> IncomingEdges => _incomingEdges;

        #endregion

    }
}

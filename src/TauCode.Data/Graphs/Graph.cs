using System;
using System.Collections.Generic;
using System.Linq;

namespace TauCode.Data.Graphs
{
    public class Graph : IGraph
    {
        #region Fields

        private readonly HashSet<INode> _nodes;

        #endregion

        #region Constructor

        public Graph()
        {
            _nodes = new HashSet<INode>();
        }

        #endregion

        #region IRhoGraph Members

        public string Name { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public void AddNode(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (_nodes.Contains(node))
            {
                throw new ArgumentException("Graph already contains this node.", nameof(node));
            }

            _nodes.Add(node);
        }

        public bool ContainsNode(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return _nodes.Contains(node);
        }

        public bool RemoveNode(INode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var removed = _nodes.Remove(node);

            return removed;
        }

        public IReadOnlyCollection<INode> Nodes => _nodes;

        public IEnumerable<IEdge> Edges => this.Nodes.SelectMany(x => x.GetOutgoingEdgesLyingInGraph(this));

        #endregion
    }
}

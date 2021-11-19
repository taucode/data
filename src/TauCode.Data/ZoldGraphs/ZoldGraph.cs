using System;
using System.Collections.Generic;
using System.Linq;

namespace TauCode.Data.ZoldGraphs
{
    public class ZoldGraph : IZoldGraph
    {
        #region Fields

        private readonly HashSet<IZoldNode> _nodes;

        #endregion

        #region Constructor

        public ZoldGraph()
        {
            _nodes = new HashSet<IZoldNode>();
        }

        #endregion

        #region IRhoGraph Members

        public string Name { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public void AddNode(IZoldNode node)
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

        public bool ContainsNode(IZoldNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            return _nodes.Contains(node);
        }

        public bool RemoveNode(IZoldNode node)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            var removed = _nodes.Remove(node);

            return removed;
        }

        public IReadOnlyCollection<IZoldNode> Nodes => _nodes;

        public IEnumerable<IZoldEdge> Edges => this.Nodes.SelectMany(x => x.GetOutgoingEdgesLyingInGraph(this));

        #endregion
    }
}

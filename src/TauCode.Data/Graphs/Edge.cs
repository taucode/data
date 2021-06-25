using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Data.Graphs
{
    [DebuggerDisplay("{From} -> {To}")]
    internal class Edge : IEdge
    {
        #region Fields

        private Node _from;
        private Node _to;
        private bool _isAlive;

        #endregion

        #region Constructor

        internal Edge(Node from, Node to)
        {
            // arg checks omitted since the type is internal.

            _from = from;
            _to = to;
            _isAlive = true;
        }

        #endregion

        #region IRhoEdge Members

        public INode From
        {
            get
            {
                this.CheckAlive();

                return _from;
            }
        }

        public INode To
        {
            get
            {
                this.CheckAlive();

                return _to;
            }
        }

        public IDictionary<string, object> Properties { get; set; }

        public void Disappear()
        {
            this.CheckAlive();

            _from.RemoveOutgoingEdge(this);
            _to.RemoveIncomingEdge(this);

            _from = null;
            _to = null;

            _isAlive = false;
        }

        #endregion

        #region Private

        private void CheckAlive()
        {
            if (!_isAlive)
            {
                throw new InvalidOperationException("Edge is not alive.");
            }
        }

        #endregion
    }
}

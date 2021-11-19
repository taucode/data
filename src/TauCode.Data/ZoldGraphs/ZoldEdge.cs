using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Data.ZoldGraphs
{
    [DebuggerDisplay("{From} -> {To}")]
    internal class ZoldEdge : IZoldEdge
    {
        #region Fields

        private ZoldNode _from;
        private ZoldNode _to;
        private bool _isAlive;

        #endregion

        #region Constructor

        internal ZoldEdge(ZoldNode from, ZoldNode to)
        {
            // arg checks omitted since the type is internal.

            _from = from;
            _to = to;
            _isAlive = true;
        }

        #endregion

        #region IRhoEdge Members

        public IZoldNode From
        {
            get
            {
                this.CheckAlive();

                return _from;
            }
        }

        public IZoldNode To
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

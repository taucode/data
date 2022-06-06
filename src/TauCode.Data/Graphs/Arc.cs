using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Data.Graphs
{
    [DebuggerDisplay("{Name} ({Tail} -> {Head})")]
    public class Arc : IArc
    {
        #region ctor

        public Arc()
        {
        }

        public Arc(string name)
        {
            this.Name = name;
        }

        #endregion

        #region IArc Members

        public string Name { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public IVertex Tail { get; internal set; }

        public IVertex Head { get; internal set; }

        public void Connect(IVertex tail, IVertex head)
        {
            if (tail == null)
            {
                throw new ArgumentNullException(nameof(tail));
            }

            if (head == null)
            {
                throw new ArgumentNullException(nameof(head));
            }

            if (this.Tail != null || this.Head != null)
            {
                throw new InvalidOperationException("Arc is not free.");
            }

            if (!(tail is Vertex tailImpl))
            {
                throw new ArgumentException($"'{nameof(tail)}' is not an instance of '{typeof(Vertex).FullName}'.", nameof(tail));
            }

            if (!(head is Vertex headImpl))
            {
                throw new ArgumentException($"'{nameof(head)}' is not an instance of '{typeof(Vertex).FullName}'.", nameof(head));
            }

            tailImpl.AddOutgoingArc(this);
            headImpl.AddIncomingArc(this);

            this.Tail = tail;
            this.Head = head;
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

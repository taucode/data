using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Data.Graphs
{
    [DebuggerDisplay("{Name} ({Tail} -> {Head})")]
    public class Edge : IEdge
    {
        #region ctor

        public Edge()
        {
        }

        public Edge(string name)
        {
            this.Name = name;
        }

        #endregion

        #region IEdge Members

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

            if (this.Tail != null)
            {
                throw new InvalidOperationException("Edge already connects vertexes.");
            }


            var tailImpl = tail as Vertex;
            if (tailImpl == null)
            {
                throw new ArgumentException($"'{nameof(tail)}' is not an instance of '{typeof(Vertex).FullName}'.", nameof(tail));
            }
            
            var headImpl = head as Vertex;
            if (headImpl == null)
            {
                throw new ArgumentException($"'{nameof(head)}' is not an instance of '{typeof(Vertex).FullName}'.", nameof(head));
            }

            tailImpl.AddOutgoingEdge(this);
            headImpl.AddIncomingEdge(this);

            this.Tail = tail;
            this.Head = head;
        }

        #endregion
    }
}

using System;

namespace TauCode.Lab.Data.Graphs
{
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

        public IVertex Tail { get; internal set; }

        public IVertex Head { get; internal set; }

        public string Name { get; set; }

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

            this.Tail = tail;
            this.Head = head;
        }

        #endregion
    }
}

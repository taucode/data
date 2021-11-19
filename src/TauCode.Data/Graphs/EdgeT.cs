namespace TauCode.Data.Graphs
{
    public class Edge<T> : Edge, IEdge<T>
    {
        #region ctor

        public Edge()
        {
        }

        public Edge(string name)
            : base(name)
        {
        }

        public Edge(string name, T data)
            : base(name)
        {
            this.Data = data;
        }

        #endregion

        #region IEdge<T> Members

        public T Data { get; set; }

        #endregion
    }
}

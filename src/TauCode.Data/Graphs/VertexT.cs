namespace TauCode.Data.Graphs
{
    public class Vertex<T> : Vertex, IVertex<T>
    {
        #region ctor

        public Vertex()
        {
        }

        public Vertex(string name)
            : base(name)
        {
        }

        public Vertex(string name, T data)
            : base(name)
        {
            this.Data = data;
        }

        #endregion

        #region IVertex<T> Members

        public T Data { get; set; }

        #endregion
    }
}

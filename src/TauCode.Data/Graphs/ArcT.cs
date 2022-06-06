namespace TauCode.Data.Graphs
{
    public class Arc<T> : Arc, IArc<T>
    {
        #region ctor

        public Arc()
        {
        }

        public Arc(string name)
            : base(name)
        {
        }

        public Arc(string name, T data)
            : base(name)
        {
            this.Data = data;
        }

        #endregion

        #region IArc<T> Members

        public T Data { get; set; }

        #endregion
    }
}

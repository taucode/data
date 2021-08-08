namespace TauCode.Lab.Data.Graphs
{
    public interface IEdge
    {
        /// <summary>
        /// Vertex from which edge starts
        /// </summary>
        IVertex Tail { get; }

        /// <summary>
        /// Vertex with which edge ends
        /// </summary>
        IVertex Head { get; }

        string Name { get; set; }

        void Connect(IVertex tail, IVertex head);
    }
}

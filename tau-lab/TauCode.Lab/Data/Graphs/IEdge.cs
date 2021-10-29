using System.Collections.Generic;

namespace TauCode.Lab.Data.Graphs
{
    public interface IEdge
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Vertex from which edge starts
        /// </summary>
        IVertex Tail { get; }

        /// <summary>
        /// Vertex with which edge ends
        /// </summary>
        IVertex Head { get; }

        void Connect(IVertex tail, IVertex head);
    }
}

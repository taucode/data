using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public interface IArc
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        /// <summary>
        /// Vertex from which arc starts
        /// </summary>
        IVertex Tail { get; }

        /// <summary>
        /// Vertex with which arc ends
        /// </summary>
        IVertex Head { get; }

        void Connect(IVertex tail, IVertex head);

        void Disconnect();
    }
}

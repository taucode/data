using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public interface IVertex
    {
        string Name { get; set; }

        IDictionary<string, object> Properties { get; set; }

        IReadOnlyCollection<IArc> OutgoingArcs { get; }

        IReadOnlyCollection<IArc> IncomingArcs { get; }
    }
}

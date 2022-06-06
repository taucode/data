using System;
using System.Collections;
using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    internal class ArcCollection : IReadOnlyCollection<IArc>
    {
        #region Fields

        private readonly HashSet<IArc> _arcs;

        #endregion

        #region ctor

        internal ArcCollection(IVertex vertex)
        {
            this.Vertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
            _arcs = new HashSet<IArc>();
        }

        #endregion

        #region Internal

        internal IVertex Vertex { get; }

        internal void AddArc(IArc arc)
        {
            _arcs.Add(arc);
        }

        internal void RemoveArc(IArc arc)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IReadOnlyCollection<IArc> Members

        public IEnumerator<IArc> GetEnumerator() => _arcs.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _arcs.GetEnumerator();

        public int Count => _arcs.Count;

        #endregion
    }
}

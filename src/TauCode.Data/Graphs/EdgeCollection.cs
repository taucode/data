﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    internal class EdgeCollection : IReadOnlyCollection<IEdge>
    {
        #region Fields

        private readonly HashSet<IEdge> _edges;

        #endregion

        #region ctor

        internal EdgeCollection(IVertex vertex)
        {
            this.Vertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
            _edges = new HashSet<IEdge>();
        }

        #endregion

        #region Internal

        internal IVertex Vertex { get; }

        internal void AddEdge(IEdge edge)
        {
            _edges.Add(edge);
        }

        internal void RemoveEdge(IEdge edge)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IReadOnlyCollection<IEdge> Members

        public IEnumerator<IEdge> GetEnumerator() => _edges.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _edges.GetEnumerator();

        public int Count => _edges.Count;

        #endregion
    }
}

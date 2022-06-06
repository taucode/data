using System;
using System.Collections;
using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public class Graph : IGraph
    {
        #region Fields

        private readonly HashSet<IVertex> _vertices;

        #endregion

        #region ctor

        public Graph()
        {
            _vertices = new HashSet<IVertex>();
        }

        #endregion

        #region Pvivate

        private void AddPrivate(IVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            if (_vertices.Contains(vertex))
            {
                throw new InvalidOperationException("Graph already contains this vertex.");
            }

            _vertices.Add(vertex);
        }

        #endregion

        #region IGraph Members

        public IEnumerator<IVertex> GetEnumerator() => _vertices.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _vertices.GetEnumerator();

        void ICollection<IVertex>.Add(IVertex vertex) => this.AddPrivate(vertex);

        public void ExceptWith(IEnumerable<IVertex> other) => _vertices.ExceptWith(other);

        public void IntersectWith(IEnumerable<IVertex> other) => _vertices.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<IVertex> other) => _vertices.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<IVertex> other) => _vertices.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<IVertex> other) => _vertices.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<IVertex> other) => _vertices.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<IVertex> other) => _vertices.Overlaps(other);

        public bool SetEquals(IEnumerable<IVertex> other) => _vertices.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<IVertex> other) => _vertices.SymmetricExceptWith(other);

        public void UnionWith(IEnumerable<IVertex> other) => _vertices.UnionWith(other);

        bool ISet<IVertex>.Add(IVertex vertex)
        {
            this.AddPrivate(vertex);
            return true;
        }

        public void Clear() => _vertices.Clear();

        public bool Contains(IVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            return _vertices.Contains(vertex);
        }

        public void CopyTo(IVertex[] array, int arrayIndex) => _vertices.CopyTo(array, arrayIndex);

        public bool Remove(IVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            return _vertices.Remove(vertex);
        }

        public int Count => _vertices.Count;

        public bool IsReadOnly => ((ICollection<IVertex>)_vertices).IsReadOnly;

        #endregion
    }
}

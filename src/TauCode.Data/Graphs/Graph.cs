using System;
using System.Collections;
using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public class Graph : IGraph
    {
        #region Fields

        private readonly HashSet<IVertex> _vertexes;

        #endregion

        #region ctor

        public Graph()
        {
            _vertexes = new HashSet<IVertex>();
        }

        #endregion

        #region Pvivate

        private void AddPrivate(IVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            if (_vertexes.Contains(vertex))
            {
                throw new InvalidOperationException("Graph already contains this vertex.");
            }

            _vertexes.Add(vertex);
        }

        #endregion

        #region IGraph Members

        public IEnumerator<IVertex> GetEnumerator() => _vertexes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _vertexes.GetEnumerator();

        void ICollection<IVertex>.Add(IVertex vertex) => this.AddPrivate(vertex);

        public void ExceptWith(IEnumerable<IVertex> other) => _vertexes.ExceptWith(other);

        public void IntersectWith(IEnumerable<IVertex> other) => _vertexes.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<IVertex> other) => _vertexes.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<IVertex> other) => _vertexes.IsProperSupersetOf(other);

        public bool IsSubsetOf(IEnumerable<IVertex> other) => _vertexes.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<IVertex> other) => _vertexes.IsSupersetOf(other);

        public bool Overlaps(IEnumerable<IVertex> other) => _vertexes.Overlaps(other);

        public bool SetEquals(IEnumerable<IVertex> other) => _vertexes.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<IVertex> other) => _vertexes.SymmetricExceptWith(other);

        public void UnionWith(IEnumerable<IVertex> other) => _vertexes.UnionWith(other);

        bool ISet<IVertex>.Add(IVertex vertex)
        {
            this.AddPrivate(vertex);
            return true;
        }

        public void Clear() => _vertexes.Clear();

        public bool Contains(IVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            return _vertexes.Contains(vertex);
        }

        public void CopyTo(IVertex[] array, int arrayIndex) => _vertexes.CopyTo(array, arrayIndex);

        public bool Remove(IVertex vertex)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            return _vertexes.Remove(vertex);
        }

        public int Count => _vertexes.Count;

        public bool IsReadOnly => ((ICollection<IVertex>)_vertexes).IsReadOnly;

        #endregion
    }
}

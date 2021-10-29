using System;
using System.Collections.Generic;
using System.Linq;

// todo: get rid of 'rho', 'node'
namespace TauCode.Lab.Data.Graphs
{
    public static class GraphExtensions
    {
        public static IEnumerable<IEdge> GetOutgoingEdgesLyingInGraph(this IVertex vertex, IGraph graph)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (!graph.Contains(vertex))
            {
                throw new InvalidOperationException("Graph does not contain this vertex.");
            }

            foreach (var outgoingEdge in vertex.OutgoingEdges)
            {
                var head = outgoingEdge.Head;

                if (graph.Contains(head))
                {
                    yield return outgoingEdge;
                }
            }
        }

        public static IEnumerable<IEdge> GetIncomingEdgesLyingInGraph(this IVertex vertex, IGraph graph)
        {
            if (vertex == null)
            {
                throw new ArgumentNullException(nameof(vertex));
            }

            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (!graph.Contains(vertex))
            {
                throw new InvalidOperationException("Graph does not contain this vertex.");
            }

            foreach (var incomingEdge in vertex.IncomingEdges)
            {
                var tail = incomingEdge.Tail;

                if (graph.Contains(tail))
                {
                    yield return incomingEdge;
                }
            }
        }

        public static IEnumerable<IEdge> GetEdges(this IGraph graph)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            return graph.SelectMany(x => x.GetOutgoingEdgesLyingInGraph(graph));
        }

        public static void CaptureNodesFrom(
            this IGraph graph,
            IGraph otherGraph,
            IEnumerable<IVertex> otherGraphNodes)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (otherGraph == null)
            {
                throw new ArgumentNullException(nameof(otherGraph));
            }

            if (otherGraphNodes == null)
            {
                throw new ArgumentNullException(nameof(otherGraphNodes));
            }

            var idx = 0;

            foreach (var otherGraphNode in otherGraphNodes)
            {
                if (otherGraphNode == null)
                {
                    throw new ArgumentException($"'{nameof(otherGraphNode)}' cannot contain nulls.");
                }

                if (graph.Contains(otherGraphNode))
                {
                    throw new ArgumentException($"Node with index {idx} already belongs to '{nameof(graph)}'.");
                }

                var captured = otherGraph.Remove(otherGraphNode);
                if (!captured)
                {
                    throw new ArgumentException($"Node with index {idx} does not belong to '{nameof(otherGraph)}'.");
                }

                graph.Add(otherGraphNode);

                idx++;
            }
        }


    }
}

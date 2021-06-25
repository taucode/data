using System;
using System.Collections.Generic;

namespace TauCode.Data.Graphs
{
    public static class GraphExtensions
    {
        public static bool IsLoop(this IEdge edge)
        {
            return edge.From == edge.To;
        }

        public static IEnumerable<IEdge> GetOutgoingEdgesLyingInGraph(this INode node, IGraph graph)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (!graph.ContainsNode(node))
            {
                throw new InvalidOperationException("Graph does not contain this node.");
            }

            foreach (var outgoingEdge in node.OutgoingEdges)
            {
                var to = outgoingEdge.To;

                if (graph.ContainsNode(to))
                {
                    yield return outgoingEdge;
                }
            }
        }

        public static IEnumerable<IEdge> GetIncomingEdgesLyingInGraph(this INode node, IGraph graph)
        {
            if (node == null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            if (!graph.ContainsNode(node))
            {
                throw new InvalidOperationException("Graph does not contain this node.");
            }

            foreach (var incomingEdge in node.IncomingEdges)
            {
                var from = incomingEdge.From;

                if (graph.ContainsNode(from))
                {
                    yield return incomingEdge;
                }
            }
        }

        public static void CaptureNodesFrom(
            this IGraph graph,
            IGraph otherGraph,
            IEnumerable<INode> otherGraphNodes)
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

                if (graph.ContainsNode(otherGraphNode))
                {
                    throw new ArgumentException($"Node with index {idx} already belongs to '{nameof(graph)}'.");
                }

                var captured = otherGraph.RemoveNode(otherGraphNode);
                if (!captured)
                {
                    throw new ArgumentException($"Node with index {idx} does not belong to '{nameof(otherGraph)}'.");
                }

                graph.AddNode(otherGraphNode);

                idx++;
            }
        }
    }
}

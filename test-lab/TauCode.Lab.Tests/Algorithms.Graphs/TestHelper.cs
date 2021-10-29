using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Lab.Data.Graphs;

// todo clean
namespace TauCode.Lab.Tests.Algorithms.Graphs
{
    internal static class TestHelper
    {
        internal static IEdge[] LinkTo(this IVertex node, params IVertex[] otherNodes)
        {
            var list = new List<IEdge>();

            foreach (var otherNode in otherNodes)
            {
                var edge = new Edge();
                edge.Connect(node, otherNode);
                list.Add(edge);
            }

            return list.ToArray();

            //return otherNodes
            //    .Select(node.DrawEdgeTo)
            //    .ToArray();
        }

        internal static IVertex GetNode(this IGraph graph, string nodeValue)
        {
            return graph.Single(x => x.Name == nodeValue);
        }

        internal static void AssertNode(
            this IGraph graph,
            IVertex node,
            IVertex[] linkedToNodes,
            IEdge[] linkedToEdges,
            IVertex[] linkedFromNodes,
            IEdge[] linkedFromEdges)
        {
            if (linkedToNodes.Length != linkedToEdges.Length)
            {
                throw new ArgumentException();
            }

            if (linkedFromNodes.Length != linkedFromEdges.Length)
            {
                throw new ArgumentException();
            }

            Assert.That(graph.Contains(node), Is.True);

            // check 'outgoing' edges
            Assert.That(node.GetOutgoingEdgesLyingInGraph(graph).Count, Is.EqualTo(linkedToNodes.Length));

            foreach (var outgoingEdge in node.GetOutgoingEdgesLyingInGraph(graph))
            {
                Assert.That(outgoingEdge.Tail, Is.EqualTo(node));

                var head = outgoingEdge.Head;
                Assert.That(graph.Contains(head), Is.True);
                Assert.That(head.IncomingEdges, Does.Contain(outgoingEdge));

                var index = Array.IndexOf(linkedToNodes, head);
                Assert.That(index, Is.GreaterThanOrEqualTo(0));
                Assert.That(outgoingEdge, Is.SameAs(linkedToEdges[index]));
            }

            // check 'incoming' edges
            Assert.That(node.GetIncomingEdgesLyingInGraph(graph).Count, Is.EqualTo(linkedFromNodes.Length));

            foreach (var incomingEdge in node.GetIncomingEdgesLyingInGraph(graph))
            {
                Assert.That(incomingEdge.Head, Is.EqualTo(node));

                var tail = incomingEdge.Tail;
                Assert.That(graph.Contains(tail), Is.True);
                Assert.That(tail.OutgoingEdges, Does.Contain(incomingEdge));

                var index = Array.IndexOf(linkedFromNodes, tail);
                Assert.That(index, Is.GreaterThanOrEqualTo(0));
                Assert.That(incomingEdge, Is.SameAs(linkedFromEdges[index]));
            }
        }

        internal static IVertex AddNamedNode(this IGraph graph, string name)
        {
            var vertex = new Vertex
            {
                Name = name,
            };

            graph.Add(vertex);
            return vertex;
        }
    }
}

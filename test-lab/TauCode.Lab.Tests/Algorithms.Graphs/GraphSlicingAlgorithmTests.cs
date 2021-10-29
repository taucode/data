using NUnit.Framework;
using System.Linq;
using TauCode.Lab.Algorithms.Graphs;
using TauCode.Lab.Data.Graphs;

namespace TauCode.Lab.Tests.Algorithms.Graphs
{
    [TestFixture]
    public class GraphSlicingAlgorithmTests : GraphTestBase
    {
        [Test]
        public void Constructor_ValidArgument_RunsOk()
        {
            // Arrange

            // Act
            var algorithm = new GraphSlicingAlgorithm();

            // Assert
        }
        
        [Test]
        public void Slice_CoupledGraph_ReturnsSlices()
        {
            // Arrange
            var a = this.Graph.AddNamedNode("a");
            var b = this.Graph.AddNamedNode("b");
            var c = this.Graph.AddNamedNode("c");
            var d = this.Graph.AddNamedNode("d");
            var e = this.Graph.AddNamedNode("e");
            var f = this.Graph.AddNamedNode("f");
            var g = this.Graph.AddNamedNode("g");
            var h = this.Graph.AddNamedNode("h");
            var i = this.Graph.AddNamedNode("i");
            var j = this.Graph.AddNamedNode("j");
            var k = this.Graph.AddNamedNode("k");
            var l = this.Graph.AddNamedNode("l");
            var m = this.Graph.AddNamedNode("m");
            var n = this.Graph.AddNamedNode("n");
            var o = this.Graph.AddNamedNode("o");
            var p = this.Graph.AddNamedNode("p");
            var q = this.Graph.AddNamedNode("q");

            d.LinkTo(a);
            e.LinkTo(f);
            g.LinkTo(e, p);
            h.LinkTo(d, e);
            i.LinkTo(a);
            j.LinkTo(f);
            k.LinkTo(c);
            l.LinkTo(g);
            m.LinkTo(n);
            n.LinkTo(m);
            o.LinkTo(j);
            p.LinkTo(l);
            q.LinkTo(i);
            
            // Act
            var algorithm = new GraphSlicingAlgorithm
            {
                Input = this.Graph,
            };

            algorithm.Run();
            var result = algorithm.Output;

            // Assert
            Assert.That(result, Has.Length.EqualTo(4));

            // 0
            CollectionAssert.AreEquivalent(
                new string[] { "a", "b", "c", "f" },
                result[0]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );
            Assert.That(result[0].GetEdges(), Is.Empty);

            // 1
            CollectionAssert.AreEquivalent(
                new string[] { "d", "e", "i", "j", "k" },
                result[1]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );
            Assert.That(result[1].GetEdges(), Is.Empty);

            // 2
            CollectionAssert.AreEquivalent(
                new string[] { "h", "o", "q" },
                result[2]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );
            Assert.That(result[2].GetEdges(), Is.Empty);

            // 3
            CollectionAssert.AreEquivalent(
                new string[] { "m", "n", "g", "l", "p" },
                result[3]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );

            Assert.That(result[3].GetEdges().ToList(), Has.Count.EqualTo(5));

            var clonedM = result[3].GetNode("m");
            var clonedN = result[3].GetNode("n");
            var clonedG = result[3].GetNode("g");
            var clonedL = result[3].GetNode("l");
            var clonedP = result[3].GetNode("p");

            var clonedEdgeMN = clonedM.GetOutgoingEdgesLyingInGraph(result[3]).Single();
            var clonedEdgeNM = clonedN.GetOutgoingEdgesLyingInGraph(result[3]).Single();
            var clonedEdgePL = clonedP.GetOutgoingEdgesLyingInGraph(result[3]).Single();
            var clonedEdgeLG = clonedL.GetOutgoingEdgesLyingInGraph(result[3]).Single();
            var clonedEdgeGP = clonedG.GetOutgoingEdgesLyingInGraph(result[3]).Single();

            result[3].AssertNode(
                clonedM,
                new IVertex[] { clonedN },
                new IEdge[] { clonedEdgeMN },
                new IVertex[] { clonedN },
                new IEdge[] { clonedEdgeNM });

            result[3].AssertNode(
                clonedN,
                new IVertex[] { clonedM },
                new IEdge[] { clonedEdgeNM },
                new IVertex[] { clonedM },
                new IEdge[] { clonedEdgeMN });

            result[3].AssertNode(
                clonedP,
                new IVertex[] { clonedL },
                new IEdge[] { clonedEdgePL },
                new IVertex[] { clonedG },
                new IEdge[] { clonedEdgeGP });

            result[3].AssertNode(
                clonedL,
                new IVertex[] { clonedG },
                new IEdge[] { clonedEdgeLG },
                new IVertex[] { clonedP },
                new IEdge[] { clonedEdgePL });

            result[3].AssertNode(
                clonedG,
                new IVertex[] { clonedP },
                new IEdge[] { clonedEdgeGP },
                new IVertex[] { clonedL },
                new IEdge[] { clonedEdgeLG });
        }

        [Test]
        public void Slice_EmptyGraph_ReturnsEmptyResult()
        {
            // Arrange

            // Act
            var algorithm = new GraphSlicingAlgorithm
            {
                Input = this.Graph,
            };

            algorithm.Run();
            var result = algorithm.Output;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public void Slice_DecoupledGraph_ReturnsResultWithSameGraph()
        {
            // Arrange
            var a = this.Graph.AddNamedNode("a");
            var b = this.Graph.AddNamedNode("b");
            var c = this.Graph.AddNamedNode("c");
            var d = this.Graph.AddNamedNode("d");
            var e = this.Graph.AddNamedNode("e");

            // Act
            var algorithm = new GraphSlicingAlgorithm
            {
                Input = this.Graph,
            };

            algorithm.Run();
            var result = algorithm.Output;

            // Assert
            Assert.That(result, Has.Length.EqualTo(1));

            // 0
            CollectionAssert.AreEquivalent(
                new string[] { "a", "b", "c", "d", "e" },
                result[0]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );
            Assert.That(result[0].GetEdges(), Is.Empty);
        }

        [Test]
        public void Slice_LightlyCoupledGraph_ReturnsResultWithSameGraph()
        {
            // Arrange
            var a = this.Graph.AddNamedNode("a");
            var b = this.Graph.AddNamedNode("b");
            var c = this.Graph.AddNamedNode("c");
            var d = this.Graph.AddNamedNode("d");
            var e = this.Graph.AddNamedNode("e");

            var w = this.Graph.AddNamedNode("w");
            var y = this.Graph.AddNamedNode("y");
            var z = this.Graph.AddNamedNode("z");

            w.LinkTo(a, b);
            y.LinkTo(d, e, a);
            z.LinkTo(e, c);

            // Act
            var algorithm = new GraphSlicingAlgorithm
            {
                Input = this.Graph
            };

            algorithm.Run();
            var result = algorithm.Output;

            // Assert
            Assert.That(result, Has.Length.EqualTo(2));

            // 0
            CollectionAssert.AreEquivalent(
                new string[] { "a", "b", "c", "d", "e" }.OrderBy(x => x),
                result[0]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );
            Assert.That(result[0].GetEdges(), Is.Empty);

            // 0
            CollectionAssert.AreEquivalent(
                new string[] { "w", "y", "z" }.OrderBy(x => x),
                result[1]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );
            Assert.That(result[1].GetEdges(), Is.Empty);

        }

        [Test]
        public void Slice_SelfReference_ReturnsValidSlices()
        {
            // Arrange
            var a = this.Graph.AddNamedNode("a");
            var b = this.Graph.AddNamedNode("b");
            var c = this.Graph.AddNamedNode("c");
            var d = this.Graph.AddNamedNode("d");
            var e = this.Graph.AddNamedNode("e");

            var w = this.Graph.AddNamedNode("w");
            var y = this.Graph.AddNamedNode("y");
            var z = this.Graph.AddNamedNode("z");

            a.LinkTo(a);
            e.LinkTo(e);

            w.LinkTo(a, b);
            y.LinkTo(d, e, a);
            z.LinkTo(e, c);

            // Act
            var algorithm = new GraphSlicingAlgorithm
            {
                Input = this.Graph
            };

            algorithm.Run();
            var result = algorithm.Output;

            // Assert
            Assert.That(result, Has.Length.EqualTo(2));

            // 0
            CollectionAssert.AreEquivalent(
                new string[] { "a", "b", "c", "d", "e" }.OrderBy(x => x),
                result[0]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );

            var edges = result[0].GetEdges().ToList();
            Assert.That(edges, Has.Count.EqualTo(2));

            var edge = edges.Single(x => x.Tail == a);
            Assert.That(edge.Head, Is.EqualTo(a));

            edge = edges.Single(x => x.Tail == e);
            Assert.That(edge.Head, Is.EqualTo(e));

            // 0
            CollectionAssert.AreEquivalent(
                new string[] { "w", "y", "z" }.OrderBy(x => x),
                result[1]
                    .Select(x => x.Name)
                    .OrderBy(x => x)
                    .ToArray()
                );
            Assert.That(result[1].GetEdges(), Is.Empty);
        }
    }
}

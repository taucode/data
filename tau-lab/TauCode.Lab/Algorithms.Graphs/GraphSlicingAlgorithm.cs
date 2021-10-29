using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TauCode.Lab.Data.Graphs;

// todo clean
namespace TauCode.Lab.Algorithms.Graphs
{
    public class GraphSlicingAlgorithm : IAlgorithm<IGraph, IReadOnlyList<IGraph>>
    {
        //private List<IGraph<T>> _result;

        private readonly IGraph _graph;
        private List<IGraph> _result;

        //public GraphSlicingAlgorithm(/*IGraph<T> graph*/)
        //{
        //    //_graph = graph ?? throw new ArgumentNullException(nameof(graph));
        //}

        private IGraph[] Slice()
        {
            if (this.Input == null)
            {
                throw new NotImplementedException();
            }

            _result = new List<IGraph>();

            while (true)
            {
                var nodes = this.GetTopLevelNodes();
                if (nodes.Count == 0)
                {
                    if (this.Input.Any())
                    {
                        _result.Add(this.Input);
                    }

                    break;
                }

                var slice = new Graph();
                slice.CaptureNodesFrom(this.Input, nodes);
                _result.Add(slice);
            }

            return _result.ToArray();
        }

        private IReadOnlyList<IVertex> GetTopLevelNodes()
        {
            var result = new List<IVertex>();

            foreach (var node in this.Input)
            {
                // todo: unoptimized. efficiency ~ O(n^2)
                var outgoingEdges = node.GetOutgoingEdgesLyingInGraph(this.Input);

                var isTopLevel = true;

                foreach (var outgoingEdge in outgoingEdges)
                {
                    if (outgoingEdge.Head == node)
                    {
                        // node referencing self, don't count - it still might be "top-level"
                        continue;
                    }

                    // node referencing another node, i.e. is not "top-level"
                    isTopLevel = false;
                    break;
                }

                if (isTopLevel)
                {
                    result.Add(node);
                }
            }

            return result;
        }

        public void Run()
        {
            var list = this.Slice();
            this.Output = list.ToArray();
        }

        public Task RunAsync(CancellationToken cancellationToken = default)
        {
            this.Run();
            return Task.CompletedTask;
        }

        public IGraph Input { get; set; }

        public IReadOnlyList<IGraph> Output { get; private set; }
    }
}

using NUnit.Framework;
using TauCode.Lab.Data.Graphs;

namespace TauCode.Lab.Tests.Algorithms.Graphs
{
    [TestFixture]
    public abstract class GraphTestBase
    {
        protected IGraph Graph { get; set; }

        [SetUp]
        public void SetUpBase()
        {
            this.Graph = new Graph();
        }

        [TearDown]
        public void TearDownBase()
        {
            this.Graph = null;
        }
    }
}

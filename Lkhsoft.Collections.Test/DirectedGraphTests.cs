using Lkhsoft.Collections.Graphs;

namespace Lkhsoft.Collections.Test;

 [TestFixture]
    public class DirectedGraphTests
    {
        [Test]
        public void AddEdge_ShouldAddDirectedEdge()
        {
            var graph = new DirectedGraph<string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B");

            Assert.That(graph.GetOutgoingEdges("A").Contains("B"), Is.True);
            Assert.That(graph.GetIncomingEdges("B").Contains("A"), Is.True);
            Assert.That(graph.GetOutgoingEdges("B").Contains("A"), Is.False);
            Assert.That(graph.GetIncomingEdges("A").Contains("B"), Is.False);
        }

        [Test]
        public void GetOutgoingEdges_ShouldReturnCorrectOutgoingEdges()
        {
            var graph = new DirectedGraph<string>
            {
                "A",
                "B",
                "C"
            };
            graph.AddEdge("A", "B");
            graph.AddEdge("A", "C");

            var outgoingEdges = graph.GetOutgoingEdges("A");

            Assert.That(outgoingEdges, Is.EqualTo(new HashSet<string> { "B", "C" }));
        }

        [Test]
        public void GetIncomingEdges_ShouldReturnCorrectIncomingEdges()
        {
            var graph = new DirectedGraph<string>
            {
                "A",
                "B",
                "C"
            };
            graph.AddEdge("A", "B");
            graph.AddEdge("C", "B");

            var incomingEdges = graph.GetIncomingEdges("B");

            Assert.That(incomingEdges, Is.EqualTo(new HashSet<string> { "A", "C" }));
        }

        [Test]
        public void Remove_ShouldRemoveNodeAndEdges()
        {
            var graph = new DirectedGraph<string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B");

            graph.Remove("A");

            Assert.That(graph.Contains("A"), Is.False);
            Assert.That(graph.GetIncomingEdges("B").Contains("A"), Is.False);
        }

        [Test]
        public void Clear_ShouldRemoveAllNodesAndEdges()
        {
            var graph = new DirectedGraph<string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B");

            graph.Clear();

            Assert.That(graph.Count, Is.EqualTo(0));
        }
    }
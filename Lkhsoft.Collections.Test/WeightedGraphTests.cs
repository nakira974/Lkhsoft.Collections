using Lkhsoft.Collections.Graphs;
using NUnit.Framework;
using System.Collections.Generic;

namespace Lkhsoft.Collections.Test
{
    [TestFixture]
    public class WeightedGraphTests
    {
        [Test]
        public void AddEdge_ShouldAddEdgeWithWeight()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            Assert.That(graph.ContainsKey("A"), Is.True);
            Assert.That(graph["A"].ContainsKey("B"), Is.True);
            Assert.That(graph["A"]["B"], Is.EqualTo(1));

            Assert.That(graph.ContainsKey("B"), Is.True);
            Assert.That(graph["B"].ContainsKey("A"), Is.True);
            Assert.That(graph["B"]["A"], Is.EqualTo(1));
        }

        [Test]
        public void Remove_ShouldRemoveNodeAndEdges()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            graph.Remove("A");

            Assert.That(graph.ContainsKey("A"), Is.False);
            Assert.That(graph.ContainsKey("B"), Is.True);
            Assert.That(graph["B"].ContainsKey("A"), Is.False);
        }

        [Test]
        public void Clear_ShouldRemoveAllNodesAndEdges()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            graph.Clear();

            Assert.That(graph.Count, Is.EqualTo(0));
        }

        [Test]
        public void ContainsKey_ShouldReturnTrueIfNodeExists()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            Assert.That(graph.ContainsKey("A"), Is.True);
        }

        [Test]
        public void ContainsKey_ShouldReturnFalseIfNodeDoesNotExist()
        {
            var graph = new WeightedGraph<int, string>();

            Assert.That(graph.ContainsKey("A"), Is.False);
        }

        [Test]
        public void TryGetValue_ShouldReturnTrueIfNodeExists()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            Assert.That(graph.TryGetValue("A", out var value), Is.True);
            Assert.That(value.ContainsKey("B"), Is.True);
            Assert.That(value["B"], Is.EqualTo(1));
        }

        [Test]
        public void TryGetValue_ShouldReturnFalseIfNodeDoesNotExist()
        {
            var graph = new WeightedGraph<int, string>();

            Assert.That(graph.TryGetValue("A", out var value), Is.False);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void Add_ShouldAddNodeAndEdges()
        {
            var graph = new WeightedGraph<int, string>();
            graph.Add("A");
            graph.Add("B");
            graph.AddEdge("A", "B", 1);

            Assert.That(graph.ContainsKey("A"), Is.True);
            Assert.That(graph["A"].ContainsKey("B"), Is.True);
            Assert.That(graph["A"]["B"], Is.EqualTo(1));

            Assert.That(graph.ContainsKey("B"), Is.True);
            Assert.That(graph["B"].ContainsKey("A"), Is.True);
            Assert.That(graph["B"]["A"], Is.EqualTo(1));
        }

        [Test]
        public void GetEnumerator_ShouldReturnEnumeratorForNodesAndEdges()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            using var enumerator = graph.GetEnumerator();
            var nodesAndEdges = new List<KeyValuePair<string, Dictionary<string, int>>>();

            while (enumerator.MoveNext())
            {
                nodesAndEdges.Add(enumerator.Current);
            }

            Assert.That(nodesAndEdges, Is.EqualTo(new[]
            {
                new KeyValuePair<string, Dictionary<string, int>>("A", new Dictionary<string, int> { { "B", 1 } }),
                new KeyValuePair<string, Dictionary<string, int>>("B", new Dictionary<string, int> { { "A", 1 } })
            }));
        }
    }
}

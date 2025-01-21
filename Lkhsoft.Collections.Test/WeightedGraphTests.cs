using Lkhsoft.Collections.Graphs;

namespace Lkhsoft.Collections.Test;

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

            Assert.That(graph.ContainsKey(1), Is.True);
            Assert.That(graph[1].Contains("A"), Is.True);
            Assert.That(graph[1].Contains("B"), Is.True);
        }

        [Test]
        public void Remove_ShouldRemoveWeightAndEdges()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            graph.Remove(1);

            Assert.That(graph.ContainsKey(1), Is.False);
        }

        [Test]
        public void Clear_ShouldRemoveAllWeightsAndEdges()
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
        public void ContainsKey_ShouldReturnTrueIfWeightExists()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            Assert.That(graph.ContainsKey(1), Is.True);
        }

        [Test]
        public void ContainsKey_ShouldReturnFalseIfWeightDoesNotExist()
        {
            var graph = new WeightedGraph<int, string>();
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            Assert.That(graph.ContainsKey(1), Is.False);
        }

        [Test]
        public void TryGetValue_ShouldReturnTrueIfWeightExists()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            Assert.That(graph.TryGetValue(1, out var value), Is.True);
            Assert.That(value.Contains("A"), Is.True);
            Assert.That(value.Contains("B"), Is.True);
        }

        [Test]
        public void TryGetValue_ShouldReturnFalseIfWeightDoesNotExist()
        {
            var graph = new WeightedGraph<int, string>();
            if (graph == null) throw new ArgumentNullException(nameof(graph));

            Assert.That(graph.TryGetValue(1, out var value), Is.False);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void Add_ShouldAddWeightAndEdges()
        {
            var graph = new WeightedGraph<int, string> {{1, new HashSet<string> { "A", "B" }}};

            Assert.That(graph.ContainsKey(1), Is.True);
            Assert.That(graph[1].Contains("A"), Is.True);
            Assert.That(graph[1].Contains("B"), Is.True);
        }

        [Test]
        public void GetEnumerator_ShouldReturnEnumeratorForWeightsAndEdges()
        {
            var graph = new WeightedGraph<int, string>
            {
                "A",
                "B"
            };
            graph.AddEdge("A", "B", 1);

            using var enumerator = graph.GetEnumerator();
            var weightsAndEdges = new List<KeyValuePair<int, ISet<string>>>();

            while (enumerator.MoveNext())
            {
                weightsAndEdges.Add(enumerator.Current);
            }

            Assert.That(weightsAndEdges, Is.EqualTo(new[] { new KeyValuePair<int, ISet<string>>(1, new HashSet<string> { "A", "B" }) }));
        }
    }
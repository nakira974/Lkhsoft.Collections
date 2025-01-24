#region

using Lkhsoft.Collections.Graphs;

#endregion

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class GraphUtilsTests
{
    [Test]
    public void BFS_ShouldReturnCorrectOrder()
    {
        var graph = new DirectedGraph<string>
        {
            "A",
            "B",
            "C",
            "D"
        };
        graph.AddEdge("A", "B");
        graph.AddEdge("A", "C");
        graph.AddEdge("B", "D");
        graph.AddEdge("C", "D");

        var result = GraphUtils.Bfs(graph, "A");

        Assert.That(result, Is.EqualTo(new List<string> {"A", "B", "C", "D"}));
    }

    [Test]
    public void DFS_ShouldReturnCorrectOrder()
    {
        var graph = new DirectedGraph<string>
        {
            "A",
            "B",
            "C",
            "D"
        };
        graph.AddEdge("A", "B");
        graph.AddEdge("A", "C");
        graph.AddEdge("B", "D");
        graph.AddEdge("C", "D");

        var result = GraphUtils.Dfs(graph, "A");

        Assert.That(result, Is.EqualTo(new List<string> {"A", "C", "D", "B"}));
    }

    [Test]
    public void Dijkstra_ShouldReturnCorrectShortestPaths()
    {
        var graph = new WeightedGraph<int, string>
        {
            "A",
            "B",
            "C",
            "D"
        };
        graph.AddEdge("A", "B", 1);
        graph.AddEdge("A", "C", 4);
        graph.AddEdge("B", "C", 2);
        graph.AddEdge("B", "D", 5);
        graph.AddEdge("C", "D", 1);

        var distances = GraphUtils.Dijkstra(graph, "A");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(distances["A"], Is.EqualTo(0));
            Assert.That(distances["B"], Is.EqualTo(1));
            Assert.That(distances["C"], Is.EqualTo(3));
            Assert.That(distances["D"], Is.EqualTo(4));
        }
    }
}
#region

using Lkhsoft.Collections.Graphs;

#endregion

namespace Lkhsoft.Collections.Test;

[TestFixture]
public class GraphTests
{
    [Test]
    public void Add_ShouldAddNodeToGraph()
    {
        var graph = new Graph<string> {"A"};

        Assert.That(graph.Contains("A"), Is.True);
    }

    [Test]
    public void AddEdge_ShouldAddEdgeBetweenNodes()
    {
        var graph = new Graph<string>
        {
            "A",
            "B"
        };
        graph.AddEdge("A", "B");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(graph.Contains("A"), Is.True);
            Assert.That(graph.Contains("B"), Is.True);
        }
    }

    [Test]
    public void Remove_ShouldRemoveNodeFromGraph()
    {
        var graph = new Graph<string>
        {
            "A",
            "B"
        };
        graph.AddEdge("A", "B");

        graph.Remove("A");

        using (Assert.EnterMultipleScope())
        {
            Assert.That(graph.Contains("A"), Is.False);
            Assert.That(graph.Contains("B"), Is.True);
        }
    }

    [Test]
    public void Clear_ShouldRemoveAllNodesFromGraph()
    {
        var graph = new Graph<string>
        {
            "A",
            "B"
        };
        graph.AddEdge("A", "B");

        graph.Clear();

        Assert.That(graph.Count, Is.EqualTo(0));
    }

    [Test]
    public void Contains_ShouldReturnTrueIfNodeExists()
    {
        var graph = new Graph<string> {"A"};

        Assert.That(graph.Contains("A"), Is.True);
    }

    [Test]
    public void Contains_ShouldReturnFalseIfNodeDoesNotExist()
    {
        var graph = new Graph<string>();
        if (graph == null) throw new ArgumentNullException(nameof(graph));

        Assert.That(graph.Contains("A"), Is.False);
    }

    [Test]
    public void CopyTo_ShouldCopyNodesToArray()
    {
        var graph = new Graph<string>
        {
            "A",
            "B"
        };

        var array = new string[2];
        graph.CopyTo(array, 0);

        Assert.That(array, Is.EqualTo(new[] {"A", "B"}));
    }

    private static readonly string[] expected = new[] {"A", "B"};

    [Test]
    public void GetEnumerator_ShouldReturnEnumeratorForNodes()
    {
        var graph = new Graph<string>
        {
            "A",
            "B"
        };

        using var enumerator = graph.GetEnumerator();
        var nodes = new List<string>();

        while (enumerator.MoveNext()) nodes.Add(enumerator.Current);

        Assert.That(nodes, Is.EqualTo(expected));
    }

    [Test]
    public void GetOutgoingEdges_ShouldReturnCorrectOutgoingEdges()
    {
        var graph = new Graph<string>
        {
            "A",
            "B",
            "C"
        };
        graph.AddEdge("A", "B");
        graph.AddEdge("A", "C");

        var outgoingEdges = graph.GetOutgoingEdges("A");

        Assert.That(outgoingEdges, Is.EqualTo(new HashSet<string> {"B", "C"}));
    }
}
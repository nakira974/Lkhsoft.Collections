namespace Lkhsoft.Collections.Graphs;

/// <summary>
/// Directed graph implementation
/// </summary>
public class DirectedGraph<TKey> : Graph<TKey> where TKey : class, IComparable<TKey>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public DirectedGraph()
    {
    }

    /// <summary>
    /// Adds an edge from one node to another
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public new void AddEdge(TKey from, TKey to)
    {
        if (base.Contains(from) && base.Contains(to)) AdjacencyList[from].Add(to);
    }

    /// <summary>
    /// Gets the outgoing edges of a node
    /// </summary>
    public new IEnumerable<TKey> GetOutgoingEdges(TKey node)
    {
        if (base.Contains(node)) return AdjacencyList[node];
        return new List<TKey>();
    }

    /// <summary>
    /// Gets the incoming edges of a node
    /// </summary>
    public IEnumerable<TKey> GetIncomingEdges(TKey node)
    {
        var incomingEdges = new List<TKey>();
        foreach (var kvp in AdjacencyList)
            if (kvp.Value.Contains(node))
                incomingEdges.Add(kvp.Key);

        return incomingEdges;
    }
}
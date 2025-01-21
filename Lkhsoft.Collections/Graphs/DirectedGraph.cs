namespace Lkhsoft.Collections.Graphs;

/// <summary>
/// Directed graph implementation
/// </summary>
public class DirectedGraph<TValue> : Graph<TValue> where TValue : class, IComparable<TValue>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public DirectedGraph() : base()
    {
    }

    /// <summary>
    /// Adds an edge from one node to another
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public new void AddEdge(TValue from, TValue to)
    {
        if (base.Contains(from) && base.Contains(to))
        {
            base._adjacencyList[from].Add(to);
        }
    }

    /// <summary>
    /// Gets the outgoing edges of a node
    /// </summary>
    public IEnumerable<TValue> GetOutgoingEdges(TValue node)
    {
        if (base.Contains(node))
        {
            return base._adjacencyList[node];
        }
        return new List<TValue>();
    }

    /// <summary>
    /// Gets the incoming edges of a node
    /// </summary>
    public IEnumerable<TValue> GetIncomingEdges(TValue node)
    {
        var incomingEdges = new List<TValue>();
        foreach (var kvp in base._adjacencyList)
        {
            if (kvp.Value.Contains(node))
            {
                incomingEdges.Add(kvp.Key);
            }
        }
        return incomingEdges;
    }
}
#region

using System.Collections;

#endregion

namespace Lkhsoft.Collections.Graphs;

/// <summary>
/// A graph is a collection of nodes and edges where each edge connects two nodes and has a weight
/// </summary>
public class WeightedGraph<TWeight, TKey> : Graph<TKey>, IDictionary<TKey, Dictionary<TKey, TWeight>>
    where TKey : class, IComparable<TKey> where TWeight : IComparable<TWeight>
{
    /// <summary>
    /// The adjacency list of the graph where the key is a node and the value is a dictionary of neighbors with their respective weights
    /// </summary>
    private readonly Dictionary<TKey, Dictionary<TKey, TWeight>> _weightedEdges;

    /// <inheritdoc/>
    public WeightedGraph()
    {
        _weightedEdges = new Dictionary<TKey, Dictionary<TKey, TWeight>>();
    }

    /// <summary>
    /// Get or set the neighbors of a node with their respective weights
    /// </summary>
    public Dictionary<TKey, TWeight> this[TKey key]
    {
        get
        {
            if (_weightedEdges.TryGetValue(key, out var item)) return item;
            throw new KeyNotFoundException();
        }
        set
        {
            if (_weightedEdges.ContainsKey(key))
                _weightedEdges[key] = new Dictionary<TKey, TWeight>(value);
            else
                _weightedEdges.Add(key, new Dictionary<TKey, TWeight>(value));
        }
    }

    /// <inheritdoc/>
    public ICollection<TKey> Keys => _weightedEdges.Keys;

    /// <inheritdoc/>
    public ICollection<Dictionary<TKey, TWeight>> Values => _weightedEdges.Values;

    /// <inheritdoc/>
    public new int Count => _weightedEdges.Count;

    /// <inheritdoc/>
    public new bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(TKey key, Dictionary<TKey, TWeight> value)
    {
        if (!_weightedEdges.ContainsKey(key)) _weightedEdges.Add(key, new Dictionary<TKey, TWeight>(value));
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return _weightedEdges.ContainsKey(key);
    }

    /// <inheritdoc/>
    public new bool Remove(TKey key)
    {
        if (!_weightedEdges.TryGetValue(key, out var neighbors)) return false;

        // Remove the key from the neighbors' dictionaries
        foreach (var neighbor in neighbors.Keys)
            if (_weightedEdges.TryGetValue(neighbor, out var neighborEdges))
                neighborEdges.Remove(key);

        // Remove the key from the adjacency list and weighted edges

        return _weightedEdges.Remove(key) && base.Remove(key);
        ;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, out Dictionary<TKey, TWeight> value)
    {
        if (_weightedEdges.TryGetValue(key, out var edge))
        {
            value = edge;
            return true;
        }

        value = null!;
        return false;
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, Dictionary<TKey, TWeight>> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public new void Clear()
    {
        _weightedEdges.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, Dictionary<TKey, TWeight>> item)
    {
        return _weightedEdges.ContainsKey(item.Key) && _weightedEdges[item.Key].Equals(item.Value);
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, Dictionary<TKey, TWeight>>[] array, int arrayIndex)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

        foreach (var kvp in _weightedEdges)
            array[arrayIndex++] = new KeyValuePair<TKey, Dictionary<TKey, TWeight>>(kvp.Key, kvp.Value);
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, Dictionary<TKey, TWeight>> item)
    {
        if (_weightedEdges.ContainsKey(item.Key) && _weightedEdges[item.Key].Equals(item.Value))
            return _weightedEdges.Remove(item.Key);
        return false;
    }

    /// <inheritdoc/>
    public new IEnumerator<KeyValuePair<TKey, Dictionary<TKey, TWeight>>> GetEnumerator()
    {
        return _weightedEdges.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Add an edge between two vertices with a specific weight
    /// </summary>
    public void AddEdge(TKey from, TKey to, TWeight weight)
    {
        if (!base.Contains(from))
            base.Add(from);
        else
            _ = base.GetOutgoingEdges(from).Add(to);

        if (!base.Contains(to))
            base.Add(to);
        else
            _ = base.GetOutgoingEdges(to).Add(from);

        if (!_weightedEdges.ContainsKey(from)) _weightedEdges[from] = new Dictionary<TKey, TWeight>();

        if (!_weightedEdges.ContainsKey(to)) _weightedEdges[to] = new Dictionary<TKey, TWeight>();

        _weightedEdges[from][to] = weight;
        _weightedEdges[to][from] = weight; // Pour un graphe non orienté
    }

    /// <summary>
    /// Get the weight of an edge between two nodes
    /// </summary>
    public TWeight GetEdgeWeight(TKey from, TKey to)
    {
        if (_weightedEdges.TryGetValue(from, out var neighbors) && neighbors.TryGetValue(to, out var weight))
            return weight;
        throw new KeyNotFoundException($"No edge between {from} and {to}.");
    }

    /// <summary>
    /// Get the neighbors of a specific node
    /// </summary>
    public new IEnumerable<TKey> GetOutgoingEdges(TKey node)
    {
        if (_weightedEdges.TryGetValue(node, out var neighbors)) return neighbors.Keys;
        return new List<TKey>();
    }
}
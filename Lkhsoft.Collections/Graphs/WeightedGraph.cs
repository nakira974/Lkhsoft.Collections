using System.Collections;
using System.Collections.Generic;

namespace Lkhsoft.Collections.Graphs
{
    /// <summary>
    /// A graph is a collection of nodes and edges where each edge connects two nodes and has a weight
    /// </summary>
    public class WeightedGraph<TWeight, TValue> : Graph<TValue>, IDictionary<TValue, Dictionary<TValue, TWeight>>
        where TValue : class, IComparable<TValue> where TWeight : IComparable<TWeight>
    {
        /// <summary>
        /// The adjacency list of the graph where the key is a node and the value is a dictionary of neighbors with their respective weights
        /// </summary>
        private readonly Dictionary<TValue, Dictionary<TValue, TWeight>> _weightedEdges;

        /// <inheritdoc/>
        public WeightedGraph()
        {
            _weightedEdges = new Dictionary<TValue, Dictionary<TValue, TWeight>>();
        }

        /// <summary>
        /// Get or set the neighbors of a node with their respective weights
        /// </summary>
        public Dictionary<TValue, TWeight> this[TValue key]
        {
            get
            {
                if (_weightedEdges.TryGetValue(key, out var item))
                {
                    return item;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                if (_weightedEdges.ContainsKey(key))
                {
                    _weightedEdges[key] = new Dictionary<TValue, TWeight>(value);
                }
                else
                {
                    _weightedEdges.Add(key, new Dictionary<TValue, TWeight>(value));
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Keys => _weightedEdges.Keys;

        /// <inheritdoc/>
        public ICollection<Dictionary<TValue, TWeight>> Values => _weightedEdges.Values;

        /// <inheritdoc/>
        public new int Count => _weightedEdges.Count;

        /// <inheritdoc/>
        public new bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(TValue key, Dictionary<TValue, TWeight> value)
        {
            if (!_weightedEdges.ContainsKey(key))
            {
                _weightedEdges.Add(key, new Dictionary<TValue, TWeight>(value));
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(TValue key)
        {
            return _weightedEdges.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Remove(TValue key)
        {
            return _weightedEdges.Remove(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TValue key, out Dictionary<TValue, TWeight> value)
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
        public void Add(KeyValuePair<TValue, Dictionary<TValue, TWeight>> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public new void Clear()
        {
            _weightedEdges.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TValue, Dictionary<TValue, TWeight>> item)
        {
            return _weightedEdges.ContainsKey(item.Key) && _weightedEdges[item.Key].Equals(item.Value);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TValue, Dictionary<TValue, TWeight>>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

            foreach (var kvp in _weightedEdges)
            {
                array[arrayIndex++] = new KeyValuePair<TValue, Dictionary<TValue, TWeight>>(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TValue, Dictionary<TValue, TWeight>> item)
        {
            if (_weightedEdges.ContainsKey(item.Key) && _weightedEdges[item.Key].Equals(item.Value))
            {
                return _weightedEdges.Remove(item.Key);
            }
            return false;
        }

        /// <inheritdoc/>
        public new IEnumerator<KeyValuePair<TValue, Dictionary<TValue, TWeight>>> GetEnumerator()
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
        public void AddEdge(TValue from, TValue to, TWeight weight)
        {
            if (!base.Contains(from))
            {
                base.Add(from);
            }
            else
            {
                _ = base.GetOutgoingEdges(from).Add(to);
            }

            if (!base.Contains(to))
            {
                base.Add(to);
            }
            else
            {
                _ = base.GetOutgoingEdges(to).Add(from);
            }

            if (!_weightedEdges.ContainsKey(from))
            {
                _weightedEdges[from] = new Dictionary<TValue, TWeight>();
            }

            if (!_weightedEdges.ContainsKey(to))
            {
                _weightedEdges[to] = new Dictionary<TValue, TWeight>();
            }

            _weightedEdges[from][to] = weight;
            _weightedEdges[to][from] = weight; // Pour un graphe non orienté
        }

        /// <summary>
        /// Get the weight of an edge between two nodes
        /// </summary>
        public TWeight GetEdgeWeight(TValue from, TValue to)
        {
            if (_weightedEdges.TryGetValue(from, out var neighbors) && neighbors.TryGetValue(to, out var weight))
            {
                return weight;
            }
            throw new KeyNotFoundException($"No edge between {from} and {to}.");
        }

        /// <summary>
        /// Get the neighbors of a specific node
        /// </summary>
        public new IEnumerable<TValue> GetOutgoingEdges(TValue node)
        {
            if (_weightedEdges.TryGetValue(node, out var neighbors))
            {
                return neighbors.Keys;
            }
            return new List<TValue>();
        }
    }
}

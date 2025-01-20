using System.Collections;

namespace Lkhsoft.Collections;

/// <summary>
/// A graph is a collection of nodes and edges where each edge connects two nodes and has a weight
/// </summary>
public class WeightedGraph<TWeight, TValue> : Graph<TValue>, IDictionary<TWeight, ISet<TValue>>
        where TValue : class, IComparable<TValue> where TWeight :  IComparable<TWeight>
{
        /// <summary>
        /// The adjacency list of the graph where the key is the weight of the edge and the value is the list of neighbors
        /// </summary>
        private readonly Dictionary<TWeight, ISet<TValue>> _weightedEdges;

        /// <inheritdoc/>
        public WeightedGraph()
        {
            _weightedEdges = new Dictionary<TWeight, ISet<TValue>>();
        }

        /// <summary>
        /// Get or set the neighbors of a node with a specific weight
        /// </summary>
        public ISet<TValue> this[TWeight key]
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
                    _weightedEdges[key] = new HashSet<TValue>(value);
                }
                else
                {
                    _weightedEdges.Add(key, new HashSet<TValue>(value));
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<TWeight> Keys => _weightedEdges.Keys;

        /// <inheritdoc/>
        public ICollection<ISet<TValue>> Values => _weightedEdges.Values;

        /// <inheritdoc/>
        public new int Count => _weightedEdges.Count;

        /// <inheritdoc/>
        public new bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(TWeight key, ISet<TValue> value)
        {
            if (!_weightedEdges.ContainsKey(key))
            {
                _weightedEdges.Add(key, new HashSet<TValue>(value));
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(TWeight key)
        {
            return _weightedEdges.ContainsKey(key);
        }

        /// <inheritdoc/>
        public bool Remove(TWeight key)
        {
            return _weightedEdges.Remove(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TWeight key, out ISet<TValue> value)
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
        public void Add(KeyValuePair<TWeight, ISet<TValue>> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public new void Clear()
        {
            _weightedEdges.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TWeight, ISet<TValue>> item)
        {
            return _weightedEdges.ContainsKey(item.Key) && _weightedEdges[item.Key].SetEquals(new HashSet<TValue>(item.Value));
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TWeight, ISet<TValue>>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

            foreach (var kvp in _weightedEdges)
            {
                array[arrayIndex++] = new KeyValuePair<TWeight, ISet<TValue>>(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TWeight, ISet<TValue>> item)
        {
            if (_weightedEdges.ContainsKey(item.Key) && _weightedEdges[item.Key].SetEquals(new HashSet<TValue>(item.Value)))
            {
                return _weightedEdges.Remove(item.Key);
            }
            return false;
        }

        /// <inheritdoc/>
        public new IEnumerator<KeyValuePair<TWeight, ISet<TValue>>> GetEnumerator()
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
            if (!base.Contains(from) || !base.Contains(to)) return;
            if (!_weightedEdges.ContainsKey(weight))
            {
                _weightedEdges[weight] = new HashSet<TValue>();
            }
            _weightedEdges[weight].Add(from);
            _weightedEdges[weight].Add(to);
        }
    }
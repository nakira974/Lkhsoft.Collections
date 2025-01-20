using System.Collections;

namespace Lkhsoft.Collections;

/// <summary>
///   A graph is a collection of nodes and edges where each edge connects two nodes
/// </summary>
 public class Graph<TValue> : ICollection<TValue> where TValue : class, IComparable<TValue>
    {
        /// <summary>
        /// The adjacency list of the graph where the key is the node and the value is the list of neighbors
        /// </summary>
        private readonly Dictionary<TValue, HashSet<TValue>> _adjacencyList;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Graph()
        {
            _adjacencyList = new Dictionary<TValue, HashSet<TValue>>();
        }

        /// <inheritdoc/>
        public virtual int Count => _adjacencyList.Count;

        /// <inheritdoc/>
        public virtual bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(TValue item)
        {
            if (!_adjacencyList.ContainsKey(item))
            {
                _adjacencyList[item] = new HashSet<TValue>();
            }
        }

        /// <summary>
        /// Add an edge between two vertices
        /// </summary>
        /// <param name="from">Node from where to start</param>
        /// <param name="to">Node to be join</param>
        public void AddEdge(TValue from, TValue to)
        {
            if (!_adjacencyList.ContainsKey(from) || !_adjacencyList.ContainsKey(to)) return;
            _adjacencyList[from].Add(to);
            _adjacencyList[to].Add(from); // Pour un graphe non orienté
        }

        /// <inheritdoc/>
        public bool Remove(TValue item)
        {
            if (!_adjacencyList.TryGetValue(item, out var value)) return false;
            foreach (var neighbor in value)
            {
                _adjacencyList[neighbor].Remove(item);
            }
            return _adjacencyList.Remove(item);
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            _adjacencyList.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(TValue item)
        {
            return _adjacencyList.ContainsKey(item);
        }

        /// <inheritdoc/>
        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

            foreach (var key in _adjacencyList.Keys)
            {
                array[arrayIndex++] = key;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<TValue> GetEnumerator()
        {
            return _adjacencyList.Keys.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
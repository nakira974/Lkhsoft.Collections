using System.Collections;

namespace Lkhsoft.Collections.Graphs;

/// <summary>
///   A graph is a collection of nodes and edges where each edge connects two nodes
/// </summary>
 public class Graph<TValue> : ICollection<TValue> where TValue : class, IComparable<TValue>
    {
        /// <summary>
        /// The adjacency list of the graph where the key is the node and the value is the list of neighbors
        /// </summary>
        private protected readonly Dictionary<TValue, HashSet<TValue>> AdjacencyList;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Graph()
        {
            AdjacencyList = new Dictionary<TValue, HashSet<TValue>>();
        }

        /// <inheritdoc/>
        public virtual int Count => AdjacencyList.Count;

        /// <inheritdoc/>
        public virtual bool IsReadOnly => false;

        /// <inheritdoc/>
        public virtual void Add(TValue item)
        {
            if (!AdjacencyList.ContainsKey(item))
            {
                AdjacencyList[item] = new HashSet<TValue>();
            }
        }

        /// <summary>
        /// Add an edge between two vertices
        /// </summary>
        /// <param name="from">Node from where to start</param>
        /// <param name="to">Node to be joint</param>
        public virtual void AddEdge(TValue from, TValue to)
        {
            if (!AdjacencyList.TryGetValue(from, out var fromValue) || !AdjacencyList.TryGetValue(to, out var toValue)) 
                return; 
            fromValue.Add(to); 
            toValue.Add(from); // Pour un graphe non orienté
        }

        /// <inheritdoc/>
        public virtual bool Remove(TValue item)
        {
            if (!AdjacencyList.TryGetValue(item, out var value)) return false;
            foreach (var neighbor in value)
            {
                AdjacencyList[neighbor].Remove(item);
            }
            return AdjacencyList.Remove(item);
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            AdjacencyList.Clear();
        }

        /// <inheritdoc/>
        public virtual bool Contains(TValue item)
        {
            return AdjacencyList.ContainsKey(item);
        }

        /// <inheritdoc/>
        public virtual void CopyTo(TValue[] array, int arrayIndex)
        {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

            foreach (var key in AdjacencyList.Keys)
            {
                array[arrayIndex++] = key;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<TValue> GetEnumerator()
        {
            return AdjacencyList.Keys.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <summary>
        /// Get the neighbors of a node
        /// </summary>
        public virtual ISet<TValue> GetOutgoingEdges(TValue node)
        {
            return AdjacencyList.TryGetValue(node, out var edges) ? edges : [];
        }
    }
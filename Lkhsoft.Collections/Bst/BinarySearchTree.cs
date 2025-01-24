using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Lkhsoft.Collections.Bst;

/// <summary>
/// Binary search tree implementation
/// </summary>
public class BinarySearchTree<T> : ICollection<T>, IXmlSerializable where T : IComparable<T>
{
    /// <summary>
    /// Binary search tree node
    /// </summary>
    private class Node
    {
        /// <summary>
        /// Node value
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// Left child
        /// </summary>
        public Node? Left { get; set; }
        
        /// <summary>
        /// Right child
        /// </summary>
        public Node? Right { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public Node(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Tree root
    /// </summary>
    private Node? _root;
    
    /// <summary>
    /// Number of elements in the tree
    /// </summary>
    private int _count;

    /// <inheritdoc/>
    public int Count => _count;
    
    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(T item)
    {
        if(Contains(item)) throw new InvalidOperationException("Item already exists");
        _root = Add(_root, item);
        _count++;
    }

    /// <summary>
    /// Add an item to the tree
    /// </summary>
    private static Node Add(Node? node, T item)
    {
        if (node is null) return new Node(item);

        var comparison = item.CompareTo(node.Value);
        switch (comparison)
        {
            case < 0:
                node.Left = Add(node.Left, item);
                break;
            case > 0:
                node.Right = Add(node.Right, item);
                break;
        }
        // Ignore duplicate items
        return node;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (!Contains(item)) return false;
        _root = Remove(_root ?? throw new InvalidOperationException("Root is null"), item);
        _count--;
        return true;
    }

    /// <summary>
    /// Removes an item from the tree
    /// </summary>
    private static Node? Remove(Node? node, T item)
    {
        if (node is null) return null;

        var comparison = item.CompareTo(node.Value);
        switch (comparison)
        {
            case < 0:
                node.Left = Remove(node.Left, item);
                break;
            case > 0:
                node.Right = Remove(node.Right, item);
                break;
            default:
            {
                if (node.Left is null) return node.Right;
                if (node.Right is null) return node.Left;

                var minLargerNode = GetMinimum(node.Right);
                node.Value = minLargerNode.Value;
                node.Right = Remove(node.Right, minLargerNode.Value);
                break;
            }
        }
        return node;
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return Contains(_root, item);
    }

    /// <summary>
    /// Checks if the tree contains an item
    /// </summary>
    private static bool Contains(Node? node, T item)
    {
        if (node is null) return false;

        var comparison = item.CompareTo(node.Value);
        return comparison switch
        {
            < 0 => Contains(node.Left, item),
            > 0 => Contains(node.Right, item),
            _ => true
        };
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _root = null;
        _count = 0;
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

        foreach (var item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return InOrderTraversal(_root ?? throw new InvalidOperationException("Root is null")).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// In-order traversal of the tree
    /// </summary>
    private static IEnumerable<T> InOrderTraversal(Node? node)
    {
        if (node is null) yield break;
        foreach (var item in InOrderTraversal(node.Left))
            yield return item;
        yield return node.Value;
        foreach (var item in InOrderTraversal(node.Right))
            yield return item;
    }

    /// <summary>
    /// Get the minimum node in the tree
    /// </summary>
    private static Node GetMinimum(Node node)
    {
        while (node.Left != null)
            node = node.Left;
        return node;
    }

    /// <inheritdoc/>
    public XmlSchema GetSchema()
    {
        return null;
    }

    /// <inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        Clear();
        reader.ReadStartElement();
        while (reader.IsStartElement("Node"))
        {
            var value = (T)new XmlSerializer(typeof(T)).Deserialize(reader)!;
            Add(value);
        }
        reader.ReadEndElement();
    }

    /// <inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
        foreach (var item in this)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, item);
        }
    }
}

/// <summary>
/// Binary search tree map implementation
/// </summary>
public class BinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue>, IXmlSerializable, IAsyncEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Binary search tree node
        /// </summary>
        private class Node(TKey key, TValue value)
        {
            /// <summary>
            /// Node key
            /// </summary>
            public TKey Key { get; set; } = key;
            
            /// <summary>
            /// Node value
            /// </summary>
            public TValue Value { get; set; } = value;
            
            /// <summary>
            /// Node left child
            /// </summary>
            public Node? Left { get; set; }
            
            /// <summary>
            /// Node right child
            /// </summary>
            public Node? Right { get; set; }
        }

        /// <summary>
        /// Tree root
        /// </summary>
        private Node? _root;
        
        /// <summary>
        /// Number of elements in the tree
        /// </summary>
        private int _count;

        /// <inheritdoc/>
        public int Count => _count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }
                throw new KeyNotFoundException();
            }
            set
            {
                if (ContainsKey(key))
                {
                    _root = UpdateValue(_root, key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>();
                InOrderTraversal(_root, (node) => keys.Add(node.Key));
                return keys;
            }
        }

        /// <inheritdoc/>
        public ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                InOrderTraversal(_root, (node) => values.Add(node.Value));
                return values;
            }
        }

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            if(ContainsKey(key)) throw new InvalidOperationException("Key already exists");
            _root = Add(_root, key, value);
            _count++;
        }

        /// <summary>
        /// Add a key-value pair to the tree
        /// </summary>
        private static Node Add(Node? node, TKey key, TValue value)
        {
            if (node is null) return new Node(key, value);

            var comparison = key.CompareTo(node.Key);
            switch (comparison)
            {
                case < 0:
                    node.Left = Add(node.Left, key, value);
                    break;
                case > 0:
                    node.Right = Add(node.Right, key, value);
                    break;
                default:
                    throw new InvalidOperationException("Key already exists");
            }
            return node;
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            if (!ContainsKey(key)) return false;
            _root = Remove(_root, key);
            _count--;
            return true;
        }

        /// <summary>
        /// Removes a key from the tree
        /// </summary>
        private static Node? Remove(Node? node, TKey key)
        {
            if (node is null) return null;

            var comparison = key.CompareTo(node.Key);
            switch (comparison)
            {
                case < 0:
                    node.Left = Remove(node.Left, key);
                    break;
                case > 0:
                    node.Right = Remove(node.Right, key);
                    break;
                default:
                {
                    if (node.Left is null) return node.Right;
                    if (node.Right is null) return node.Left;

                    var minLargerNode = GetMinimum(node.Right);
                    node.Key = minLargerNode.Key;
                    node.Value = minLargerNode.Value;
                    node.Right = Remove(node.Right, minLargerNode.Key);
                    break;
                }
            }
            return node;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return ContainsKey(_root, key);
        }

        /// <summary>
        /// Checks if the tree contains a key
        /// </summary>
        private static bool ContainsKey(Node? node, TKey key)
        {
            if (node is null) return false;

            var comparison = key.CompareTo(node.Key);
            return comparison switch
            {
                < 0 => ContainsKey(node.Left, key),
                > 0 => ContainsKey(node.Right, key),
                _ => true
            };
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value)
        {
            var node = FindNode(_root, key);
            if (node is not null)
            {
                value = node.Value;
                return true;
            }
            value = default!;
            return false;
        }

        /// <summary>
        /// Find a node in the tree
        /// </summary>
        private static Node? FindNode(Node? node, TKey key)
        {
            if (node is null) return null;

            var comparison = key.CompareTo(node.Key);
            return comparison switch
            {
                < 0 => FindNode(node.Left, key),
                > 0 => FindNode(node.Right, key),
                _ => node
            };
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

            int index = arrayIndex;
            InOrderTraversal(_root, (node) => array[index++] = new KeyValuePair<TKey, TValue>(node.Key, node.Value));
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return InOrderTraversal(_root).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// In-order traversal of the tree
        /// </summary>
        private static IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(Node? node)
        {
            if (node is null) yield break;
            foreach (var item in InOrderTraversal(node.Left))
                yield return item;
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            foreach (var item in InOrderTraversal(node.Right))
                yield return item;
        }

        /// <summary>
        /// In-order traversal of the tree
        /// </summary>
        private static void InOrderTraversal(Node? node, Action<Node> action)
        {
            if (node is null) return;
            InOrderTraversal(node.Left, action);
            action(node);
            InOrderTraversal(node.Right, action);
        }

        /// <summary>
        /// Gets the minimum node in the tree
        /// </summary>
        private static Node GetMinimum(Node node)
        {
            while (node.Left is not null)
                node = node.Left;
            return node;
        }

        /// <summary>
        /// Updates the value of a key in the tree
        /// </summary>
        private static Node? UpdateValue(Node? node, TKey key, TValue value)
        {
            if (node is null) return null;

            var comparison = key.CompareTo(node.Key);
            switch (comparison)
            {
                case 0:
                    node.Value = value;
                    break;
                case < 0:
                    node.Left = UpdateValue(node.Left, key, value);
                    break;
                default:
                    node.Right = UpdateValue(node.Right, key, value);
                    break;
            }
            return node;
        }

        /// <inheritdoc/>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <inheritdoc/>
        public void ReadXml(XmlReader reader)
        {
            Clear();
            reader.ReadStartElement();
            while (reader.IsStartElement("Node"))
            {
                var key = (TKey)new XmlSerializer(typeof(TKey)).Deserialize(reader)!;
                var value = (TValue)new XmlSerializer(typeof(TValue)).Deserialize(reader)!;
                Add(key, value);
            }
            reader.ReadEndElement();
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var item in this)
            {
                new XmlSerializer(typeof(TKey)).Serialize(writer, item.Key);
                new XmlSerializer(typeof(TValue)).Serialize(writer, item.Value);
            }
        }

        /// <inheritdoc/>
        public IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return InOrderTraversalAsync(_root, cancellationToken).GetAsyncEnumerator(cancellationToken);
        }

        /// <summary>
        /// Asynchronously traverses the tree in-order
        /// </summary>
        private static async IAsyncEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversalAsync(Node? node, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (node is null) yield break;
            await foreach (var item in InOrderTraversalAsync(node.Left, cancellationToken))
                yield return item;
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            await foreach (var item in InOrderTraversalAsync(node.Right, cancellationToken))
                yield return item;
        }
    }
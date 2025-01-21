using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace Lkhsoft.Collections.Bst;

/// <summary>
/// AVL tree implementation
/// </summary>
public class AvlTree<T> : ICollection<T>, IXmlSerializable where T : class, IComparable<T>, IXmlSerializable
    {
        /// <summary>
        /// Node of the AVL tree
        /// </summary>
        private class AvlNode
        {
            /// <summary>
            /// Value of the node
            /// </summary>
            public T Value { get; set; }
            
            /// <summary>
            /// Height of the node
            /// </summary>
            public int Height { get; set; }
            
            /// <summary>
            /// Left child node
            /// </summary>
            public AvlNode? Left { get; set; }
            
            /// <summary>
            /// Right child node
            /// </summary>
            public AvlNode? Right { get; set; }

            /// <summary>
            /// Base constructor
            /// </summary>
            public AvlNode(T value)
            {
                Value = value;
                Height = 1;
            }
        }

        /// <summary>
        /// Root node of the tree
        /// </summary>
        private AvlNode? _root;
        
        /// <summary>
        /// Number of elements in the collection
        /// </summary>
        private int _count;

        /// <summary>
        /// Gets the number of elements in the collection
        /// </summary>
        public int Count => _count;
        
        /// <summary>
        /// Gets a value indicating whether the collection is read-only
        /// </summary>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(T item)
        {
            _root = Add(_root, item);
            _count++;
        }

        /// <summary>
        /// Adds the given item to the given subtree
        /// </summary>
        private AvlNode Add(AvlNode? node, T item)
        {
            if (node is null) return new AvlNode(item);

            var comparison = item.CompareTo(node.Value);
            switch (comparison)
            {
                case < 0:
                    node.Left = Add(node.Left, item);
                    break;
                case > 0:
                    node.Right = Add(node.Right, item);
                    break;
                default:
                    throw new InvalidOperationException("Duplicate items are not allowed in an AVL tree.");
            }

            UpdateHeight(node);
            return Balance(node);
        }

        /// <summary>
        /// Removes the node with the given value from the tree
        /// </summary>
        public bool Remove(T item)
        {
            if (!Contains(item)) return false;
            _root = Remove(_root, item);
            _count--;
            return true;
        }

        /// <summary>
        /// Removes the node with the given value from the given subtree
        /// </summary>
        private AvlNode? Remove(AvlNode? node, T item)
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
                    if (minLargerNode is not null)
                    {
                        node.Value = minLargerNode.Value;
                        node.Right = Remove(node.Right, minLargerNode.Value);
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected error while removing a node from the AVL tree");
                    }

                    break;
                }
            }

            UpdateHeight(node);
            return Balance(node);
        }
        
        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return FindNode(_root, item) is not null;
        }

        /// <summary>
        /// Finds the node with the given value in the given subtree
        /// </summary>
        private AvlNode? FindNode(AvlNode? node, T item)
        {
            while (node is not null)
            {
                var comparison = item.CompareTo(node.Value);
                switch (comparison)
                {
                    case < 0:
                        node = node.Left;
                        break;
                    case > 0:
                        node = node.Right;
                        break;
                    default:
                        return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the minimum node in the given subtree
        /// </summary>
        private AvlNode? GetMinimum(AvlNode avlNode)
        {
            while (avlNode.Left is not null)
                avlNode = avlNode.Left;
            return avlNode;
        }

        /// <summary>
        /// Updates the height of the given node
        /// </summary>
        private void UpdateHeight(AvlNode avlNode)
        {
            avlNode.Height = 1 + Math.Max(GetHeight(avlNode.Left), GetHeight(avlNode.Right));
        }

        /// <summary>
        /// Gets the height of the given node
        /// </summary>
        private int GetHeight(AvlNode? node)
        {
            return node?.Height ?? 0;
        }

        /// <summary>
        ///  Gets the balance of the given node
        /// </summary>
        private int GetBalance(AvlNode avlNode)
        {
            return GetHeight(avlNode.Left) - GetHeight(avlNode.Right);
        }

        /// <summary>
        /// Balances the given node
        /// </summary>
        private AvlNode Balance(AvlNode avlNode)
        {
            var balance = GetBalance(avlNode);

            switch (balance)
            {
                case > 1:
                {
                    if (GetBalance(avlNode.Left ?? throw new InvalidOperationException("Error while balancing left node")) < 0)
                        avlNode.Left = RotateLeft(avlNode.Left);
                    return RotateRight(avlNode);
                }
                case < -1:
                {
                    if (GetBalance(avlNode.Right!) > 0)
                        avlNode.Right = RotateRight(avlNode.Right ?? throw new InvalidOperationException("Error while balancing right node"));
                    return RotateLeft(avlNode);
                }
                default:
                    return avlNode;
            }
        }

        /// <summary>
        ///  Rotates the given node to the left
        /// </summary>
        private AvlNode RotateLeft(AvlNode avlNode)
        {
            var newRoot = avlNode.Right ?? throw new InvalidOperationException("Error while rotating left node");

            avlNode.Right = newRoot.Left;
            newRoot.Left = avlNode;
            UpdateHeight(avlNode);
            UpdateHeight(newRoot);
            return newRoot;
        }

        /// <summary>
        ///   Rotates the given node to the right
        /// </summary>
        private AvlNode RotateRight(AvlNode avlNode)
        {
            var newRoot = avlNode.Left ?? throw new InvalidOperationException("Error while rotating right node");
            
            avlNode.Left = newRoot.Right;
            newRoot.Right = avlNode;
            UpdateHeight(avlNode);
            UpdateHeight(newRoot);
            return newRoot;
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
            if (arrayIndex < 0 || arrayIndex > array.Length)
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count)
                throw new ArgumentException("The target array is too small.");

            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        /// <inheritdoc/>

        public IEnumerator<T> GetEnumerator()
        {
            return InOrderTraversal(_root).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Traverses the tree in in-order
        /// </summary>
        private IEnumerable<T> InOrderTraversal(AvlNode? node)
        {
            if (node is null) yield break;
            foreach (var item in InOrderTraversal(node.Left))
                yield return item;
            yield return node.Value;
            foreach (var item in InOrderTraversal(node.Right))
                yield return item;
        }

        /// <inheritdoc/>
        public System.Xml.Schema.XmlSchema? GetSchema()
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
                Add(value ?? throw new InvalidOperationException("Error while reading XML"));
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
    /// AVL tree map implementation
    /// </summary>
    public class AvlTree<TKey, TValue> : IDictionary<TKey, TValue>, IXmlSerializable where TKey : class, IComparable<TKey>, IXmlSerializable where TValue : class, IXmlSerializable
    {
        private class Node(TKey key, TValue value)
        {
            /// <summary>
            /// Key of the node
            /// </summary>
            public TKey Key { get; set; } = key;
            
            /// <summary>
            /// Value of the node
            /// </summary>
            public TValue Value { get; set; } = value;
            
            /// <summary>
            /// Height of the node
            /// </summary>
            public int Height { get; set; } = 1;
            
            /// <summary>
            /// Left child node
            /// </summary>
            public Node? Left { get; set; }
            
            /// <summary>
            /// Right child node
            /// </summary>
            public Node? Right { get; set; }
        }

        /// <summary>
        /// Root node of the tree
        /// </summary>
        private Node? _root;
        
        /// <summary>
        /// Number of elements in the collection
        /// </summary>
        private int _count;

        /// <summary>
        /// Gets the value associated with the given key
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                var node = FindNode(_root, key);
                if (node is null) throw new KeyNotFoundException($"Key '{key}' not found.");
                return node.Value;
            }
            set => _root = AddOrUpdate(_root, key, value);
        }

        /// <summary>
        /// Gets the keys of the tree
        /// </summary>
        public ICollection<TKey> Keys => GetKeys();
        
        /// <summary>
        /// Gets the values of the tree
        /// </summary>
        public ICollection<TValue> Values => GetValues();

        /// <summary>
        /// Gets the number of elements in the collection
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only
        /// </summary>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(TKey key, TValue value)
        {
            _root = Add(_root, key, value);
            _count++;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return FindNode(_root, key) is not null;
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            if (!ContainsKey(key)) return false;
            _root = Remove(_root, key);
            _count--;
            return true;
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

            value = null!;
            return false;
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach (var kvp in this)
                array[arrayIndex++] = kvp;
        }

        /// <summary>
        /// Removes the given key-value pair from the tree
        /// </summary>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Contains(item) && Remove(item.Key);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _root = null;
            _count = 0;
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
        /// Adds the given key-value pair to the tree
        /// </summary>
        private Node Add(Node? node, TKey key, TValue value)
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
                    throw new ArgumentException($"Duplicate key '{key}'.");
            }

            UpdateHeight(node);
            return Balance(node);
        }

        /// <summary>
        /// Adds the given key-value pair to the given subtree
        /// </summary>
        private Node AddOrUpdate(Node? node, TKey key, TValue value)
        {
            if (node is null) return new Node(key, value);

            var comparison = key.CompareTo(node.Key);
            switch (comparison)
            {
                case < 0:
                    node.Left = AddOrUpdate(node.Left, key, value);
                    break;
                case > 0:
                    node.Right = AddOrUpdate(node.Right, key, value);
                    break;
                default:
                    node.Value = value;
                    break;
            }

            UpdateHeight(node);
            return Balance(node);
        }

        /// <summary>
        /// Removes the node with the given key from the tree
        /// </summary>
        private Node? Remove(Node? node, TKey key)
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

            UpdateHeight(node);
            return Balance(node);
        }

        /// <summary>
        /// Finds the node with the given key in the given subtree
        /// </summary>
        private Node? FindNode(Node? node, TKey key)
        {
            while (node is not null)
            {
                var comparison = key.CompareTo(node.Key);
                switch (comparison)
                {
                    case < 0:
                        node = node.Left;
                        break;
                    case > 0:
                        node = node.Right;
                        break;
                    default:
                        return node;
                }
            }

            return null;
        }

        /// <summary>
        /// Traverses the tree in in-order
        /// </summary>
        private IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(Node? node)
        {
            if (node is null) yield break;
            foreach (var kvp in InOrderTraversal(node.Left))
                yield return kvp;

            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);

            foreach (var kvp in InOrderTraversal(node.Right))
                yield return kvp;
        }

        /// <summary>
        /// Gets the keys of the tree
        /// </summary>
        private ICollection<TKey> GetKeys()
        {
            var keys = new List<TKey>();
            foreach (var kvp in this)
                keys.Add(kvp.Key);
            return keys;
        }

        /// <summary>
        /// Gets the values of the tree
        /// </summary>
        private ICollection<TValue> GetValues()
        {
            var values = new List<TValue>();
            foreach (var kvp in this)
                values.Add(kvp.Value);
            return values;
        }

        /// <summary>
        /// Gets the minimum node in the given subtree
        /// </summary>
        private Node GetMinimum(Node node)
        {
            while (node.Left is not null)
                node = node.Left;
            return node;
        }

        /// <summary>
        /// Updates the height of the given node
        /// </summary>
        private void UpdateHeight(Node node)
        {
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));
        }

        /// <summary>
        /// Gets the height of the given node
        /// </summary>
        private int GetHeight(Node? node)
        {
            return node?.Height ?? 0;
        }

        /// <summary>
        /// Gets the balance of the given node
        /// </summary>
        private int GetBalance(Node node)
        {
            return GetHeight(node.Left) - GetHeight(node.Right);
        }

        /// <summary>
        /// Balances the given node
        /// </summary>
        private Node Balance(Node node)
        {
            var balance = GetBalance(node);

            switch (balance)
            {
                case > 1:
                {
                    if (GetBalance(node.Left!) < 0)
                        node.Left = RotateLeft(node.Left ?? throw new InvalidOperationException("Error while balancing left node"));
                    return RotateRight(node);
                }
                case < -1:
                {
                    if (GetBalance(node.Right!) > 0)
                        node.Right = RotateRight(node.Right ?? throw new InvalidOperationException("Error while balancing right node"));
                    return RotateLeft(node);
                }
                default:
                    return node;
            }
        }

        /// <summary>
        /// Rotates the given node to the left
        /// </summary>
        private Node RotateLeft(Node node)
        {
            var newRoot = node.Right!;
            node.Right = newRoot.Left;
            newRoot.Left = node;
            UpdateHeight(node);
            UpdateHeight(newRoot);
            return newRoot;
        }

        /// <summary>
        /// Rotates the given node to the right
        /// </summary>
        private Node RotateRight(Node node)
        {
            var newRoot = node.Left!;
            node.Left = newRoot.Right;
            newRoot.Right = node;
            UpdateHeight(node);
            UpdateHeight(newRoot);
            return newRoot;
        }
        
        /// <inheritdoc/>
        public System.Xml.Schema.XmlSchema? GetSchema()
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
                var keySerializer = new XmlSerializer(typeof(TKey));
                var valueSerializer = new XmlSerializer(typeof(TValue));

                var key = (TKey)keySerializer.Deserialize(reader)!;
                var value = (TValue)valueSerializer.Deserialize(reader)!;

                Add(key, value);
            }
            reader.ReadEndElement();
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer)
        {
            foreach (var kvp in this)
            {
                var keySerializer = new XmlSerializer(typeof(TKey));
                var valueSerializer = new XmlSerializer(typeof(TValue));

                writer.WriteStartElement("Node");
                keySerializer.Serialize(writer, kvp.Key);
                valueSerializer.Serialize(writer, kvp.Value);
                writer.WriteEndElement();
            }
        }
    }
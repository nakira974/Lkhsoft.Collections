#region

using System.Collections;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace Lkhsoft.Collections.Bst;

/// <summary>
/// Red-black tree implementation
/// </summary>
[JsonConverter(typeof(RedBlackTreeJsonConverterFactory))]
public class RedBlackTree<T> : ICollection<T>, IXmlSerializable, IAsyncEnumerable<T> where T : class, IComparable<T>
{
    /// <summary>
    ///  Root node of the tree
    /// </summary>
    private Node? _root;

    /// <summary>
    ///  Count of the tree
    /// </summary>
    private int _count;

    /// <summary>
    ///  Node class for Red-Black Tree
    /// </summary>
    private class Node
    {
        /// <summary>
        /// Value of the node
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Color of the node
        /// </summary>
        public NodeColor Color { get; set; }

        /// <summary>
        /// Node's children and parent
        /// </summary>
        public Node? Left { get; set; }

        /// <summary>
        ///  Node's children and parent
        /// </summary>
        public Node? Right { get; set; }

        /// <summary>
        ///  Node's children and parent
        /// </summary>
        public Node? Parent { get; set; }

        /// <summary>
        ///  Node base constructor
        /// </summary>
        /// <param name="value">Node's value</param>
        /// <param name="nodeColor">Node color</param>
        public Node(T value, NodeColor nodeColor = NodeColor.Red)
        {
            Value = value;
            Color = nodeColor;
        }
    }

    /// <summary>
    /// Node color for the red-black tree
    /// </summary>
    private enum NodeColor
    {
        /// <summary>
        /// Red color
        /// </summary>
        Red = 0xF00,

        /// <summary>
        /// Black color
        /// </summary>
        Black = 0xFFF
    }


    /// <summary>
    ///  Count of the collection
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Is the collection read only ?
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Default constructor
    /// </summary>
    public RedBlackTree()
    {
    }

    /// <summary>
    /// Add an item to the tree
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void Add(T item)
    {
        if (_root is null)
            _root = new Node(item) {Color = NodeColor.Black};
        else
            Add(_root, item);
        _count++;
    }

    /// <summary>
    ///  Internal add function helper
    /// </summary>
    /// <param name="node">Node to be added</param>
    /// <param name="item">Item to be added</param>
    private void Add(Node? node, T item)
    {
        if (node is null) return;
        var comparison = node.Value.CompareTo(item);
        if (comparison < 0)
        {
            if (node?.Left is null)
            {
                if (node is not null)
                {
                    node.Left = new Node(item)
                    {
                        Parent = node
                    };
                    FixTree(node.Left);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add(node.Left, item);
            }
        }
        else
        {
            if (node?.Right is null)
            {
                if (node is not null)
                {
                    node.Right = new Node(item)
                    {
                        Parent = node
                    };
                    FixTree(node.Right);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add(node.Right, item);
            }
        }
    }

    /// <summary>
    ///  Fix the tree after adding a node
    /// </summary>
    /// <param name="node">Node from where to start fixing the tree</param>
    private void FixTree(Node? node)
    {
        while (node?.Parent is not null && node != _root && node.Parent.Color.Equals(NodeColor.Red))
            if (node.Parent == node.Parent.Parent?.Left)
            {
                var uncle = node.Parent.Parent.Right;
                if (uncle is not null && uncle.Color.Equals(NodeColor.Red))
                {
                    node.Parent.Color = NodeColor.Black;
                    uncle.Color = NodeColor.Black;
                    node.Parent.Parent.Color = NodeColor.Red;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Right)
                    {
                        node = node.Parent;
                        RotateLeft(node);
                    }

                    if (node.Parent is not null)
                    {
                        node.Parent.Color = NodeColor.Black;
                        if (node.Parent.Parent is not null)
                        {
                            node.Parent.Parent.Color = NodeColor.Red;
                            RotateRight(node.Parent.Parent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Parent of the parent is null");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Parent is null");
                    }
                }
            }
            else
            {
                var uncle = node.Parent.Parent?.Left;
                if (uncle is not null && uncle.Color.Equals(NodeColor.Red))
                {
                    node.Parent.Color = NodeColor.Black;
                    uncle.Color = NodeColor.Black;
                    if (node.Parent.Parent is not null)
                    {
                        node.Parent.Parent.Color = NodeColor.Red;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        throw new InvalidOperationException("Parent of the parent is null");
                    }
                }
                else
                {
                    if (node == node.Parent.Left)
                    {
                        node = node.Parent;
                        RotateRight(node);
                    }

                    if (node.Parent is not null)
                    {
                        node.Parent.Color = NodeColor.Black;
                        if (node.Parent.Parent is not null)
                        {
                            node.Parent.Parent.Color = NodeColor.Red;
                            RotateLeft(node.Parent.Parent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Parent of the parent is null");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Parent is null");
                    }
                }
            }

        if (_root is not null) _root.Color = NodeColor.Black;
        else throw new InvalidOperationException("Root is null");
    }

    /// <summary>
    /// Rotate the tree to the left
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateLeft(Node? node)
    {
        var temp = node?.Right;
        if (node is not null && temp is not null)
        {
            node.Right = temp.Left;
            if (temp.Left is not null) temp.Left.Parent = node;

            temp.Parent = node.Parent;
            if (node.Parent is null)
                _root = temp;
            else if (node == node.Parent.Left)
                node.Parent.Left = temp;
            else
                node.Parent.Right = temp;

            temp.Left = node;
            node.Parent = temp;
        }
        else
        {
            throw new InvalidOperationException("Node or temp is null");
        }
    }

    /// <summary>
    /// Rotate the tree to the right
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateRight(Node? node)
    {
        var temp = node?.Left;
        if (node is not null && temp is not null)
        {
            node.Left = temp.Right;
            if (temp.Right is not null) temp.Right.Parent = node;

            temp.Parent = node.Parent;
            if (node.Parent is null)
                _root = temp;
            else if (node == node.Parent.Right)
                node.Parent.Right = temp;
            else
                node.Parent.Left = temp;

            temp.Right = node;
            node.Parent = temp;
        }
        else
        {
            throw new InvalidOperationException("Node or temp is null");
        }
    }

    /// <summary>
    /// Removes an item from the tree
    /// </summary>
    /// <param name="item">Item to be removed</param>
    /// <returns>True if the item was deleted, false otherwise</returns>
    public bool Remove(T item)
    {
        var node = FindNode(item);
        if (node is null) return false;

        if (node.Left is not null && node.Right is not null)
        {
            var temp = GetMinimum(node.Right);
            if (temp is null) throw new InvalidOperationException("Temp is null");
            node.Value = temp.Value;
            node = temp;
        }

        var child = node.Right ?? node.Left;
        if (child is not null)
        {
            child.Parent = node.Parent;
            if (node.Parent is null)
                _root = child;
            else if (node == node.Parent.Left)
                node.Parent.Left = child;
            else
                node.Parent.Right = child;
        }
        else if (node.Parent is null)
        {
            _root = null;
        }
        else
        {
            if (node == node.Parent.Left)
                node.Parent.Left = null;
            else
                node.Parent.Right = null;
            FixTree(node);
        }

        _count--;
        return true;
    }

    /// <summary>
    /// Finds a node in the tree
    /// </summary>
    /// <param name="item">Item to be found</param>
    /// <returns>The node containing the item</returns>
    private Node? FindNode(T item)
    {
        var current = _root;
        while (current is not null)
        {
            var comparison = current.Value.CompareTo(item);
            switch (comparison)
            {
                case < 0:
                    current = current.Left;
                    break;
                case > 0:
                    current = current.Right;
                    break;
                default:
                    return current;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the minimum node in the tree
    /// </summary>
    /// <param name="node">Node from where to start</param>
    /// <returns>The minimum node of the current branch</returns>
    private Node? GetMinimum(Node? node)
    {
        while (node?.Left is not null) node = node.Left;
        return node;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _root = null;
        _count = 0;
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return FindNode(item) is not null;
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array is null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

        foreach (var item in this) array[arrayIndex++] = item;
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
    /// Order traversal of the tree
    /// </summary>
    /// <param name="node">Node from where to start</param>
    /// <returns>The current values of traversed nodes</returns>
    private IEnumerable<T> InOrderTraversal(Node? node)
    {
        if (node is null) yield break;
        foreach (var item in InOrderTraversal(node.Left)) yield return item;
        yield return node.Value;
        foreach (var item in InOrderTraversal(node.Right)) yield return item;
    }

    /// <summary>
    /// Transforms the tree into an array
    /// </summary>
    /// <returns>The current tree as an array</returns>
    public T[] ToArray()
    {
        var array = new T[Count];
        CopyTo(array, 0);
        return array;
    }

    /// <summary>
    /// Transforms the tree into a list
    /// </summary>
    /// <returns>The current tree as a list</returns>
    public List<T> ToList()
    {
        return [..this];
    }

    /// <summary>
    ///  Get the XML schema
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Serialization context</param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Count", _count);
        info.AddValue("Items", ToArray());
    }

    /// <summary>
    /// Deserialize the tree from XML
    /// </summary>
    /// <param name="reader">XmlReader containing tree as string</param>
    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement();
        _count = int.Parse(reader.GetAttribute("Count") ?? throw new InvalidOperationException("Count is null"));
        var items = new T[_count];
        for (var i = 0; i < _count; i++)
        {
            reader.ReadStartElement("Item");
            var itemSerializer = new XmlSerializer(typeof(T));
            items[i] = (T) itemSerializer.Deserialize(reader)! ?? throw new InvalidOperationException("Item is null");
            reader.ReadEndElement();
        }

        reader.ReadEndElement();

        foreach (var item in items) Add(item);
    }

    /// <summary>
    /// Serialize the tree to XML
    /// </summary>
    /// <param name="writer">XmlWriter to write in</param>
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Count", _count.ToString());
        foreach (var item in this)
        {
            writer.WriteStartElement("Item");
            var itemSerializer = new XmlSerializer(typeof(T));
            itemSerializer.Serialize(writer, item);
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Gets the XML schema of the tree
    /// </summary>
    /// <returns>Tree's XML schema</returns>
    public XmlSchema GetSchema()
    {
        return null;
    }

    /// <summary>
    /// Gets the async enumerator of the tree
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An async enumerator</returns>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new RedBlackTreeAsyncEnumerator(_root, cancellationToken);
    }

    /// <summary>
    /// Async enumerator for the tree
    /// </summary>
    private class RedBlackTreeAsyncEnumerator : IAsyncEnumerator<T>
    {
        /// <summary>
        /// Root node of the tree
        /// </summary>
        private readonly Node? _root;

        /// <summary>
        /// CancellationToken for the enumerator
        /// </summary>
        private readonly CancellationToken _cancellationToken;

        /// <summary>
        /// Stack for the enumerator
        /// </summary>
        private readonly Stack<Node?> _stack;

        /// <summary>
        /// Current node of the enumerator
        /// </summary>
        private Node? _currentNode;

        /// <summary>
        /// Base constructor
        /// </summary>
        /// <param name="root">Tree from where to start the async iteration</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public RedBlackTreeAsyncEnumerator(Node? root, CancellationToken cancellationToken)
        {
            _root = root;
            _cancellationToken = cancellationToken;
            _stack = new Stack<Node?>();
            _currentNode = null;
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public ValueTask<bool> MoveNextAsync()
        {
            if (_currentNode is null)
            {
                _currentNode = _root;
                while (_currentNode is not null)
                {
                    _stack.Push(_currentNode);
                    _currentNode = _currentNode.Left;
                }
            }

            if (_stack.Count > 0)
            {
                _currentNode = _stack.Pop();
                if (_currentNode is not null)
                {
                    var rightNode = _currentNode.Right;
                    while (rightNode is not null)
                    {
                        _stack.Push(rightNode);
                        rightNode = rightNode.Left;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Right node is null");
                }

                return ValueTask.FromResult(true);
            }

            return ValueTask.FromResult(false);
        }

        /// <inheritdoc/>
        public T Current => _currentNode?.Value;
    }
}

/// <summary>
/// Red-black tree JSON converter
/// </summary>
/// <typeparam name="T">Stored type inside the tree</typeparam>
public class RedBlackTreeJsonConverter<T> : JsonConverter<RedBlackTree<T>> where T : class, IComparable<T>
{
    /// <inheritdoc/>
    public override RedBlackTree<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tree = new RedBlackTree<T>();
        var count = 0;

        if (reader.TokenType != JsonTokenType.StartArray) throw new JsonException();

        while (reader.Read())
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    if (count > tree.Count) throw new JsonException("JSON count is different from the actual count");
                    return tree;
                case JsonTokenType.PropertyName:
                    var propertyName = reader.GetString();

                    switch (propertyName)
                    {
                        // Si la propriété est "Count", on peut l'ignorer.
                        case "Count":
                            count = JsonSerializer.Deserialize<int>(ref reader, options);
                            break;
                        // Si la propriété est "Items", on la traite comme une liste d'objets
                        case "Items":
                        {
                            if (reader.TokenType == JsonTokenType.PropertyName)
                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonTokenType.EndArray) break;

                                    if (reader.TokenType != JsonTokenType.StartObject) continue;
                                    T value = null!;

                                    while (reader.Read())
                                    {
                                        if (reader.TokenType == JsonTokenType.EndObject) break;

                                        if (reader.TokenType != JsonTokenType.PropertyName) continue;
                                        var innerPropertyName = reader.GetString();
                                        reader.Read(); // Passer à la valeur

                                        switch (innerPropertyName)
                                        {
                                            case "Value":
                                                value = JsonSerializer.Deserialize<T>(ref reader, options) ??
                                                        throw new InvalidOperationException("Value is null");
                                                break;
                                        }
                                    }

                                    tree.Add(value);
                                }

                            break;
                        }
                    }

                    break;
            }


        throw new JsonException("Unexpected end of JSON");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RedBlackTree<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("Count", value.Count);

        writer.WriteStartArray("Items");
        foreach (var item in value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, item, options);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}

/// <summary>
/// Red-black tree JSON converter factory
/// </summary>
public class RedBlackTreeJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeof(RedBlackTree<>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition())) return false;

        var itemType = typeToConvert.GetGenericArguments()[0];
        return typeof(IComparable<>).MakeGenericType(itemType).IsAssignableFrom(itemType);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(RedBlackTreeJsonConverter<>).MakeGenericType(itemType);
        return (JsonConverter) Activator.CreateInstance(converterType)! ??
               throw new InvalidOperationException("Converter is null");
    }
}

/// <summary>
/// Red-black tree map implementation
/// </summary>
[JsonConverter(typeof(RedBlackTreeMapJsonConverterFactory))]
public class RedBlackTree<TKey, TValue> : IDictionary<TKey, TValue>, IXmlSerializable,
    IAsyncEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : class, IComparable<TKey>
{
    /// <summary>
    /// Root node of the tree
    /// </summary>
    private Node? _root;

    /// <summary>
    /// Count of the tree
    /// </summary>
    private int _count;

    /// <summary>
    /// Node class for Red-Black Tree
    /// </summary>
    private class Node(TKey key, TValue value, NodeColor nodeColor = NodeColor.Red)
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
        /// Color of the node
        /// </summary>
        public NodeColor Color { get; set; } = nodeColor;

        /// <summary>
        /// Node's children and parent
        /// </summary>
        public Node? Left { get; set; }

        /// <summary>
        /// Node's children and parent
        /// </summary>
        public Node? Right { get; set; }

        /// <summary>
        /// Node's children and parent
        /// </summary>
        public Node? Parent { get; set; }
    }

    /// <summary>
    /// Node color for the red-black tree
    /// </summary>
    private enum NodeColor
    {
        Red = 0xF00,
        Black = 0xFFF
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    /// <summary>
    /// Root node of the tree
    /// </summary>
    public int Count => _count;

    /// <summary>
    /// Is the collection read only ?
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Default constructor
    /// </summary>
    public RedBlackTree()
    {
    }

    /// <summary>
    /// Get or set the value of the tree
    /// </summary>
    public TValue this[TKey key]
    {
        get
        {
            var node = FindNode(key);
            if (node is null) throw new KeyNotFoundException();
            return node.Value;
        }
        set
        {
            var node = FindNode(key);
            if (node is null)
                Add(key, value);
            else
                node.Value = value;
        }
    }

    /// <summary>
    /// Keys of the tree
    /// </summary>
    public ICollection<TKey> Keys => this.Select(kvp => kvp.Key).ToList();

    /// <summary>
    /// Values of the tree
    /// </summary>
    public ICollection<TValue> Values => this.Select(kvp => kvp.Value).ToList();

    /// <inheritdoc/>
    public void Add(TKey key, TValue value)
    {
        if (_root is null)
            _root = new Node(key, value) {Color = NodeColor.Black};
        else
            Add(_root, key, value);
        _count++;
    }

    /// <summary>
    /// Internal add function helper
    /// </summary>
    private void Add(Node? node, TKey key, TValue value)
    {
        if (node is null) return;
        var comparison = node.Key.CompareTo(key);
        if (comparison < 0)
        {
            if (node?.Left is null)
            {
                if (node is not null)
                {
                    node.Left = new Node(key, value)
                    {
                        Parent = node
                    };
                    FixTree(node.Left);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add(node.Left, key, value);
            }
        }
        else
        {
            if (node?.Right is null)
            {
                if (node is not null)
                {
                    node.Right = new Node(key, value)
                    {
                        Parent = node
                    };
                    FixTree(node.Right);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add(node.Right, key, value);
            }
        }
    }

    /// <summary>
    /// Fix the tree after adding a node
    /// </summary>
    private void FixTree(Node? node)
    {
        while (node?.Parent is not null && node != _root && node.Parent.Color.Equals(NodeColor.Red))
            if (node.Parent == node.Parent.Parent?.Left)
            {
                var uncle = node.Parent.Parent.Right;
                if (uncle is not null && uncle.Color.Equals(NodeColor.Red))
                {
                    node.Parent.Color = NodeColor.Black;
                    uncle.Color = NodeColor.Black;
                    node.Parent.Parent.Color = NodeColor.Red;
                    node = node.Parent.Parent;
                }
                else
                {
                    if (node == node.Parent.Right)
                    {
                        node = node.Parent;
                        RotateLeft(node);
                    }

                    if (node.Parent is not null)
                    {
                        node.Parent.Color = NodeColor.Black;
                        if (node.Parent.Parent is not null)
                        {
                            node.Parent.Parent.Color = NodeColor.Red;
                            RotateRight(node.Parent.Parent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Parent of the parent is null");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Parent is null");
                    }
                }
            }
            else
            {
                var uncle = node.Parent.Parent?.Left;
                if (uncle is not null && uncle.Color.Equals(NodeColor.Red))
                {
                    node.Parent.Color = NodeColor.Black;
                    uncle.Color = NodeColor.Black;
                    if (node.Parent.Parent is not null)
                    {
                        node.Parent.Parent.Color = NodeColor.Red;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        throw new InvalidOperationException("Parent of the parent is null");
                    }
                }
                else
                {
                    if (node == node.Parent.Left)
                    {
                        node = node.Parent;
                        RotateRight(node);
                    }

                    if (node.Parent is not null)
                    {
                        node.Parent.Color = NodeColor.Black;
                        if (node.Parent.Parent is not null)
                        {
                            node.Parent.Parent.Color = NodeColor.Red;
                            RotateLeft(node.Parent.Parent);
                        }
                        else
                        {
                            throw new InvalidOperationException("Parent of the parent is null");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("Parent is null");
                    }
                }
            }

        if (_root is not null) _root.Color = NodeColor.Black;
        else throw new InvalidOperationException("Root is null");
    }

    /// <summary>
    /// Rotate the tree to the left
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateLeft(Node? node)
    {
        var temp = node?.Right;
        if (node is not null && temp is not null)
        {
            node.Right = temp.Left;
            if (temp.Left is not null) temp.Left.Parent = node;

            temp.Parent = node.Parent;
            if (node.Parent is null)
                _root = temp;
            else if (node == node.Parent.Left)
                node.Parent.Left = temp;
            else
                node.Parent.Right = temp;

            temp.Left = node;
            node.Parent = temp;
        }
        else
        {
            throw new InvalidOperationException("Node or temp is null");
        }
    }

    /// <summary>
    /// Rotate the tree to the right
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateRight(Node? node)
    {
        var temp = node?.Left;
        if (node is not null && temp is not null)
        {
            node.Left = temp.Right;
            if (temp.Right is not null) temp.Right.Parent = node;

            temp.Parent = node.Parent;
            if (node.Parent is null)
                _root = temp;
            else if (node == node.Parent.Right)
                node.Parent.Right = temp;
            else
                node.Parent.Left = temp;

            temp.Right = node;
            node.Parent = temp;
        }
        else
        {
            throw new InvalidOperationException("Node or temp is null");
        }
    }

    /// <inheritdoc/>
    public bool Remove(TKey key)
    {
        var node = FindNode(key);
        if (node is null) return false;

        if (node.Left is not null && node.Right is not null)
        {
            var temp = GetMinimum(node.Right);
            if (temp is null) throw new InvalidOperationException("Temp is null");
            node.Key = temp.Key;
            node.Value = temp.Value;
            node = temp;
        }

        var child = node.Right ?? node.Left;
        if (child is not null)
        {
            child.Parent = node.Parent;
            if (node.Parent is null)
                _root = child;
            else if (node == node.Parent.Left)
                node.Parent.Left = child;
            else
                node.Parent.Right = child;
        }
        else if (node.Parent is null)
        {
            _root = null;
        }
        else
        {
            if (node == node.Parent.Left)
                node.Parent.Left = null;
            else
                node.Parent.Right = null;
            FixTree(node);
        }

        _count--;
        return true;
    }

    /// <summary>
    /// Find a node in the tree
    /// </summary>
    private Node? FindNode(TKey key)
    {
        var current = _root;
        while (current is not null)
        {
            var comparison = current.Key.CompareTo(key);

            if (comparison < 0)
                current = current.Left;
            else if (comparison > 0)
                current = current.Right;
            else
                return current;
        }

        return null;
    }

    /// <summary>
    /// Get the minimum node in the tree
    /// </summary>
    private Node? GetMinimum(Node? node)
    {
        while (node?.Left is not null) node = node.Left;
        return node;
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
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
        return FindNode(item.Key) is not null;
    }

    /// <inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return FindNode(key) is not null;
    }

    /// <inheritdoc/>
    public bool TryGetValue(TKey key, out TValue value)
    {
        var node = FindNode(key);
        if (node is null)
        {
            value = default!;
            return false;
        }

        value = node.Value;
        return true;
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

        foreach (var item in this) array[arrayIndex++] = item;
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
    /// In order traversal of the tree
    /// </summary>
    private IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(Node? node)
    {
        if (node is null) yield break;
        foreach (var item in InOrderTraversal(node.Left)) yield return item;
        yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        foreach (var item in InOrderTraversal(node.Right)) yield return item;
    }

    /// <summary>
    /// Get the tree as an array
    /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Count", _count);
        info.AddValue("Items", this.ToArray());
    }

    /// <inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        var count = int.Parse(reader.GetAttribute("Count") ?? throw new InvalidOperationException("Count is null"));

        reader.ReadStartElement();
        var items = new KeyValuePair<TKey, TValue>[count];
        for (var i = 0; i < count; i++)
        {
            reader.ReadStartElement("Item");
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));
            var key = (TKey) keySerializer.Deserialize(reader)! ?? throw new InvalidOperationException("Key is null");
            var value = (TValue) valueSerializer.Deserialize(reader)! ??
                        throw new InvalidOperationException("Value is null");
            reader.ReadEndElement();
            items[i] = new KeyValuePair<TKey, TValue>(key, value);
        }

        reader.ReadEndElement();

        foreach (var item in items) Add(item.Key, item.Value);

        if (count != _count) throw new InvalidOperationException("XML count is different from the actual count");
    }

    /// <inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Count", _count.ToString());
        foreach (var item in this)
        {
            writer.WriteStartElement("Item");
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));
            keySerializer.Serialize(writer, item.Key);
            valueSerializer.Serialize(writer, item.Value);
            writer.WriteEndElement();
        }
    }

    /// <inheritdoc/>
    public XmlSchema GetSchema()
    {
        return null;
    }

    /// <inheritdoc/>
    public IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        return new RedBlackTreeAsyncEnumerator(_root, cancellationToken);
    }

    /// <summary>
    /// Async enumerator for the tree map
    /// </summary>
    private class RedBlackTreeAsyncEnumerator(Node? root, CancellationToken cancellationToken)
        : IAsyncEnumerator<KeyValuePair<TKey, TValue>>
    {
        private readonly CancellationToken _cancellationToken = cancellationToken;
        private readonly Stack<Node?> _stack = new();
        private Node? _currentNode = null;

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public ValueTask<bool> MoveNextAsync()
        {
            if (_currentNode is null)
            {
                _currentNode = root;
                while (_currentNode is not null)
                {
                    _stack.Push(_currentNode);
                    _currentNode = _currentNode.Left;
                }
            }

            if (_stack.Count > 0)
            {
                _currentNode = _stack.Pop();
                if (_currentNode is not null)
                {
                    var rightNode = _currentNode.Right;
                    while (rightNode is not null)
                    {
                        _stack.Push(rightNode);
                        rightNode = rightNode.Left;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Right node is null");
                }

                return ValueTask.FromResult(true);
            }

            return ValueTask.FromResult(false);
        }

        /// <inheritdoc/>
        public KeyValuePair<TKey, TValue> Current =>
            new(_currentNode?.Key ?? throw new InvalidOperationException("Current node is null"),
                _currentNode.Value ?? throw new InvalidOperationException("Current node is null"));
    }
}

/// <summary>
///  Red-black tree map JSON converter
/// </summary>
public class RedBlackTreeJsonConverter<TKey, TValue> : JsonConverter<RedBlackTree<TKey, TValue>>
    where TKey : class, IComparable<TKey>
{
    /// <inheritdoc/>
    public override RedBlackTree<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var tree = new RedBlackTree<TKey, TValue>();
        var count = 0;

        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException("Expected start of object.");

        while (reader.Read())
            switch (reader.TokenType)
            {
                case JsonTokenType.EndObject:
                    if (count > tree.Count) throw new JsonException("JSON count is different from the actual count");
                    return tree;
                case JsonTokenType.PropertyName:
                    var propertyName = reader.GetString();

                    switch (propertyName)
                    {
                        // Si la propriété est "Count", on peut l'ignorer.
                        case "Count":
                            count = JsonSerializer.Deserialize<int>(ref reader, options);
                            break;
                        // Si la propriété est "Items", on la traite comme une liste d'objets
                        case "Items":
                        {
                            if (reader.TokenType == JsonTokenType.PropertyName)
                                while (reader.Read())
                                {
                                    if (reader.TokenType == JsonTokenType.EndArray) break;

                                    if (reader.TokenType != JsonTokenType.StartObject) continue;
                                    TKey key = default!;
                                    TValue value = default!;

                                    while (reader.Read())
                                    {
                                        if (reader.TokenType == JsonTokenType.EndObject) break;

                                        if (reader.TokenType != JsonTokenType.PropertyName) continue;
                                        var innerPropertyName = reader.GetString();
                                        reader.Read(); // Passer à la valeur

                                        switch (innerPropertyName)
                                        {
                                            case "Key":
                                                key = JsonSerializer.Deserialize<TKey>(ref reader, options) ??
                                                      throw new InvalidOperationException("Key is null");
                                                break;
                                            case "Value":
                                                value = JsonSerializer.Deserialize<TValue>(ref reader, options) ??
                                                        throw new InvalidOperationException("Value is null");
                                                break;
                                        }
                                    }

                                    tree.Add(key, value);
                                }

                            break;
                        }
                    }

                    break;
            }

        throw new JsonException("Unexpected end of JSON");
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RedBlackTree<TKey, TValue> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("Count", value.Count);

        writer.WriteStartArray("Items");
        foreach (var item in value)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Key");
            JsonSerializer.Serialize(writer, item.Key, options);
            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, item.Value, options);
            writer.WriteEndObject();
        }

        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}

/// <summary>
///  Red-black tree map JSON converter factory
/// </summary>
public class RedBlackTreeMapJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeof(RedBlackTree<,>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition())) return false;

        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];
        return typeof(IComparable<>).MakeGenericType(keyType).IsAssignableFrom(keyType);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];
        var converterType = typeof(RedBlackTreeJsonConverter<,>).MakeGenericType(keyType, valueType);
        return (JsonConverter) Activator.CreateInstance(converterType)! ??
               throw new InvalidOperationException("Converter is null");
    }
}
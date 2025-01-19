using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Lkhsoft.Collections;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Red-black tree implementation with serialization support for XML and JSON
/// </summary>
/// <typeparam name="T"></typeparam>
[JsonConverter(typeof(RedBlackTreeJsonConverterFactory))]
public class RedBlackTree<T> : ICollection<T>, IXmlSerializable, IAsyncEnumerable<T> where T : class, IComparable<T>, IEquatable<T>, IXmlSerializable
{
    /// <summary>
    ///  Root node of the tree
    /// </summary>
    private Node? root;
    
    /// <summary>
    ///  Comparer function for the tree
    /// </summary>
    private Func<T, int> comparer;
    
    /// <summary>
    ///  Count of the tree
    /// </summary>
    private int count;
    
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
        public bool IsRed { get; set; }
        
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
        /// <param name="isRed">Is node red ?</param>
        public Node(T value, bool isRed = true)
        {
            Value = value;
            IsRed = isRed;
        }
    }
    
    
    /// <summary>
    ///  Count of the collection
    /// </summary>
    public int Count => count;

    /// <summary>
    /// Is the collection read only ?
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Default constructor
    /// </summary>
    public RedBlackTree() : this(null)
    {
        
    }

    /// <summary>
    /// Default constructor with comparer
    /// </summary>
    /// <param name="comparer">Comparer function</param>
    public RedBlackTree(Func<T, int>? comparer)
    {
        this.comparer = comparer ?? (x => x.CompareTo(default(T)));
    }
    
    /// <summary>
    /// Add an item to the tree
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void Add(T item)
    {
        if (root is null)
        {
            root = new Node(item) { IsRed = false };
        }
        else
        {
            Add(root, item);
        }
        count++;
    }

    /// <summary>
    ///  Internal add function helper
    /// </summary>
    /// <param name="node">Node to be added</param>
    /// <param name="item">Item to be added</param>
    private void Add(Node? node, T item)
    {
        var comparison = comparer(item);
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
        while (node?.Parent is not null && node != root && node.Parent.IsRed)
        {
            if (node.Parent == node.Parent.Parent?.Left)
            {
                var uncle = node.Parent.Parent.Right;
                if (uncle is not null && uncle.IsRed)
                {
                    node.Parent.IsRed = false;
                    uncle.IsRed = false;
                    node.Parent.Parent.IsRed = true;
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
                        node.Parent.IsRed = false;
                        if (node.Parent.Parent is not null)
                        {
                            node.Parent.Parent.IsRed = true;
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
                if (uncle is not null && uncle.IsRed)
                {
                    node.Parent.IsRed = false;
                    uncle.IsRed = false;
                    if (node.Parent.Parent is not null)
                    {
                        node.Parent.Parent.IsRed = true;
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
                        node.Parent.IsRed = false;
                        if (node.Parent.Parent is not null)
                        {
                            node.Parent.Parent.IsRed = true;
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
        }

        if (root is not null) root.IsRed = false;
        else throw new InvalidOperationException("Root is null");
    }

    /// <summary>
    /// Rotate the tree to the left
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateLeft(Node? node)
    {
        var temp = node?.Right;
        if (node is not null)
        {
            node.Right = temp?.Left;
            if (temp?.Left is not null)
            {
                temp.Left.Parent = node;
            }

            if (temp is not null)
            {
                temp.Parent = node.Parent;
                if (node.Parent is not null)
                {
                    root = temp;
                }
                else if (node == node.Parent?.Left)
                {
                    node.Parent.Left = temp;
                }
                else
                {
                    if (node.Parent is not null) node.Parent.Right = temp;
                    else throw new InvalidOperationException("Parent is null");
                }

                temp.Left = node;
                node.Parent = temp;
            }
            else
            {
                throw new InvalidOperationException("Temp is null");
            }
        }else throw new InvalidOperationException("Node is null");
    }

    /// <summary>
    /// Rotate the tree to the right
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateRight(Node? node)
    {
        var temp = node?.Left;
        if (node is not null)
        {
            node.Left = temp?.Right;
            if (temp?.Right is not null)
            {
                temp.Right.Parent = node;
            }

            if (temp is not null)
            {
                temp.Parent = node.Parent;
                if (node.Parent is null)
                {
                    root = temp;
                }
                else if (node == node.Parent.Right)
                {
                    node.Parent.Right = temp;
                }
                else
                {
                    node.Parent.Left = temp;
                }

                temp.Right = node;
                node.Parent = temp;
            }
            else
            {
                throw new InvalidOperationException("Temp is null");
            }
        }
        else
        {
            throw new InvalidOperationException("Node is null");
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
        if (node is null)
        {
            return false;
        }

        if (node.Left is not null && node.Right is not null)
        {
            var temp = GetMinimum(node.Right);
            if(temp is null) throw new InvalidOperationException("Temp is null");
            node.Value = temp.Value;
            node = temp;
        }

        var child = node.Right ?? node.Left;
        if (child is not null)
        {
            child.Parent = node.Parent;
            if (node.Parent is null)
            {
                root = child;
            }
            else if (node == node.Parent.Left)
            {
                node.Parent.Left = child;
            }
            else
            {
                node.Parent.Right = child;
            }
        }
        else if (node.Parent is null)
        {
            root = null;
        }
        else
        {
            if (node == node.Parent.Left)
            {
                node.Parent.Left = null;
            }
            else
            {
                node.Parent.Right = null;
            }
            FixTree(node);
        }

        count--;
        return true;
    }

    /// <summary>
    /// Finds a node in the tree
    /// </summary>
    /// <param name="item">Item to be found</param>
    /// <returns>The node containing the item</returns>
    private Node? FindNode(T item)
    {
        var current = root;
        while (current is not null)
        {
            int comparison = comparer(item);
            if (comparison < 0)
            {
                current = current.Left;
            }
            else if (comparison > 0)
            {
                current = current.Right;
            }
            else
            {
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
        while (node?.Left is not null)
        {
            node = node.Left;
        }
        return node;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        root = null;
        count = 0;
    }

    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return FindNode(item) is not null;
    }
    
    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (array is null)
        {
            throw new ArgumentNullException(nameof(array));
        }
        if (arrayIndex < 0 || arrayIndex > array.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }
        if (array.Length - arrayIndex < Count)
        {
            throw new ArgumentException("Array is too small");
        }

        foreach (T item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return InOrderTraversal(root).GetEnumerator();
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
        if (node is not null)
        {
            foreach (T item in InOrderTraversal(node.Left))
            {
                yield return item;
            }
            yield return node.Value;
            foreach (T item in InOrderTraversal(node.Right))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Transforms the tree into an array
    /// </summary>
    /// <returns>The current tree as an array</returns>
    public T[] ToArray()
    {
        T[] array = new T[Count];
        CopyTo(array, 0);
        return array;
    }

    /// <summary>
    /// Transforms the tree into a list
    /// </summary>
    /// <returns>The current tree as a list</returns>
    public List<T> ToList()
    {
        return new List<T>(this);
    }
    
    /// <summary>
    ///  Get the XML schema
    /// </summary>
    /// <param name="info">Serialization info</param>
    /// <param name="context">Serialization context</param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Count", count);
        info.AddValue("Items", this.ToArray());
    }

    /// <summary>
    /// Deserialize the tree from XML
    /// </summary>
    /// <param name="reader">XmlReader containing tree as string</param>
    public void ReadXml(XmlReader reader)
    {
        reader.ReadStartElement();
        count = int.Parse(reader.GetAttribute("Count") ?? throw new InvalidOperationException("Count is null"));
        T[] items = new T[count];
        for (int i = 0; i < count; i++)
        {
            reader.ReadStartElement("Item");
            XmlSerializer itemSerializer = new XmlSerializer(typeof(T));
            items[i] = (T)itemSerializer.Deserialize(reader)! ?? throw new InvalidOperationException("Item is null");
            reader.ReadEndElement();
        }
        reader.ReadEndElement();

        foreach (T item in items)
        {
            Add(item);
        }
    }

    /// <summary>
    /// Serialize the tree to XML
    /// </summary>
    /// <param name="writer">XmlWriter to write in</param>
    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Count", count.ToString());
        foreach (T item in this)
        {
            writer.WriteStartElement("Item");
            XmlSerializer itemSerializer = new XmlSerializer(typeof(T));
            itemSerializer.Serialize(writer, item);
            writer.WriteEndElement();
        }
    }

    /// <summary>
    /// Gets the XML schema of the tree
    /// </summary>
    /// <returns>Tree's XML schema</returns>
    public System.Xml.Schema.XmlSchema GetSchema()
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
        return new RedBlackTreeAsyncEnumerator(root, cancellationToken);
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
        private Stack<Node?> _stack;
        
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
            this._root = root;
            this._cancellationToken = cancellationToken;
            this._stack = new Stack<Node?>();
            this._currentNode = null;
        }

        /// <inheritdoc/>
        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc/>
        public async ValueTask<bool> MoveNextAsync()
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

                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public T Current => _currentNode?.Value;
    }
}

/// <summary>
/// Red-black tree JSON converter
/// </summary>
/// <typeparam name="T">Stored type inside the tree</typeparam>
public class RedBlackTreeJsonConverter<T> : JsonConverter<RedBlackTree<T>> where T : class, IComparable<T>, IEquatable<T>, IXmlSerializable
{
    /// <inheritdoc/>
    public override RedBlackTree<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tree = new RedBlackTree<T>();
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return tree;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                tree.Add(item ?? throw new InvalidOperationException("Item is null"));
            }
        }

        throw new JsonException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, RedBlackTree<T> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var item in value)
        {
            JsonSerializer.Serialize(writer, item, options);
        }
        writer.WriteEndArray();
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
        if (!typeof(RedBlackTree<>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition()))
        {
            return false;
        }

        var itemType = typeToConvert.GetGenericArguments()[0];
        return typeof(IComparable<>).MakeGenericType(itemType).IsAssignableFrom(itemType) &&
               typeof(IEquatable<>).MakeGenericType(itemType).IsAssignableFrom(itemType) &&
               typeof(IXmlSerializable).IsAssignableFrom(itemType);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(RedBlackTreeJsonConverter<>).MakeGenericType(itemType);
        return (JsonConverter)Activator.CreateInstance(converterType)! ?? throw new InvalidOperationException("Converter is null");
    }
}
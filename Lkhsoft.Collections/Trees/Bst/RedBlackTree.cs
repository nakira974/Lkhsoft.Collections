#region

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace Lkhsoft.Collections.Trees.Bst;

/// <summary>
/// Red-black tree implementation
/// </summary>
[JsonConverter(typeof(RedBlackTreeJsonConverterFactory))]
public class RedBlackTree<T> : BinarySearchTree<T> where T : IComparable<T>
{
    /// <summary>
    ///  Node class for Red-Black Tree
    /// </summary>
    private class RedBlackTreeNode : BinarySearchTreeNode
    {
        /// <summary>
        /// Color of the node
        /// </summary>
        public NodeColor Color { get; set; }

        /// <summary>
        ///  Node's children and parent
        /// </summary>
        public RedBlackTreeNode? Parent { get; set; }

        /// <summary>
        ///  Node base constructor
        /// </summary>
        /// <param name="value">Node's value</param>
        /// <param name="nodeColor">Node color</param>
        public RedBlackTreeNode(T value, NodeColor nodeColor = NodeColor.Red) : base(value)
        {
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
    /// Default constructor
    /// </summary>
    public RedBlackTree()
    {
    }

    /// <summary>
    /// Add an item to the tree
    /// </summary>
    /// <param name="item">Item to be added</param>
    public override void Add(T item)
    {
        if (Root is null)
        {
            Root = new RedBlackTreeNode(item) {Color = NodeColor.Black};
        }
        else
        {
            if (Contains(item)) throw new InvalidOperationException("Item already exists");
            Add((RedBlackTreeNode?) Root, item);
        }

        _count++;
    }

    /// <summary>
    ///  Internal add function helper
    /// </summary>
    /// <param name="node">Node to be added</param>
    /// <param name="item">Item to be added</param>
    private void Add(RedBlackTreeNode? node, T item)
    {
        if (node is null) return;
        var comparison = node.Value.CompareTo(item);
        if (comparison < 0)
        {
            if (node?.Left is null)
            {
                if (node is not null)
                {
                    node.Left = new RedBlackTreeNode(item)
                    {
                        Parent = node
                    };
                    FixTree((RedBlackTreeNode?) node.Left);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add((RedBlackTreeNode?) node.Left, item);
            }
        }
        else
        {
            if (node?.Right is null)
            {
                if (node is not null)
                {
                    node.Right = new RedBlackTreeNode(item)
                    {
                        Parent = node
                    };
                    FixTree((RedBlackTreeNode?) node.Right);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add((RedBlackTreeNode?) node.Right, item);
            }
        }
    }

    /// <summary>
    ///  Fix the tree after adding a node
    /// </summary>
    /// <param name="node">Node from where to start fixing the tree</param>
    private void FixTree(RedBlackTreeNode? node)
    {
        while (node?.Parent is not null && node != Root && node.Parent.Color.Equals(NodeColor.Red))
            if (node.Parent == node.Parent.Parent?.Left)
            {
                var uncle = (RedBlackTreeNode?) node.Parent.Parent.Right;
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
                var uncle = (RedBlackTreeNode?) node.Parent.Parent?.Left;
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

        if (Root is not null)
            ((RedBlackTreeNode) Root).Color = NodeColor.Black;
        else throw new InvalidOperationException("Root is null");
    }

    /// <summary>
    /// Rotate the tree to the left
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateLeft(RedBlackTreeNode? node)
    {
        var temp = node?.Right;
        if (node is not null && temp is not null)
        {
            node.Right = temp.Left;
            if (temp.Left is not null) ((RedBlackTreeNode) temp.Left).Parent = node;

            ((RedBlackTreeNode) temp).Parent = node.Parent;
            if (node.Parent is null)
                Root = temp;
            else if (node == node.Parent.Left)
                node.Parent.Left = temp;
            else
                node.Parent.Right = temp;

            temp.Left = node;
            node.Parent = (RedBlackTreeNode?) temp;
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
    private void RotateRight(RedBlackTreeNode? node)
    {
        var temp = (RedBlackTreeNode?) node?.Left;
        if (node is not null && temp is not null)
        {
            node.Left = temp.Right;
            if (temp.Right is not null) ((RedBlackTreeNode) temp.Right).Parent = node;

            temp.Parent = node.Parent;
            if (node.Parent is null)
                Root = temp;
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
    public override bool Remove(T item)
    {
        var node = FindNode(item);
        if (node is null) return false;

        if (node.Left is not null && node.Right is not null)
        {
            var temp = GetMinimum((RedBlackTreeNode?) node.Right);
            if (temp is null) throw new InvalidOperationException("Temp is null");
            node.Value = temp.Value;
            node = temp;
        }

        var child = node.Right ?? node.Left;
        if (child is not null)
        {
            ((RedBlackTreeNode) child).Parent = node.Parent;
            if (node.Parent is null)
                Root = child;
            else if (node == node.Parent.Left)
                node.Parent.Left = child;
            else
                node.Parent.Right = child;
        }
        else if (node.Parent is null)
        {
            Root = null;
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
    private RedBlackTreeNode? FindNode(T item)
    {
        var current = Root;
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
                    return (RedBlackTreeNode?) current;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets the minimum node in the tree
    /// </summary>
    /// <param name="node">Node from where to start</param>
    /// <returns>The minimum node of the current branch</returns>
    private RedBlackTreeNode? GetMinimum(RedBlackTreeNode? node)
    {
        while (node?.Left is not null) node = (RedBlackTreeNode?) node.Left;
        return node;
    }

    /// <inheritdoc/>
    public override bool Contains(T item)
    {
        return FindNode(item) is not null;
    }

    /// <inheritdoc/>
    public override void CopyTo(T[] array, int arrayIndex)
    {
        if (array is null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

        foreach (var item in this) array[arrayIndex++] = item;
    }

    /// <inheritdoc/>
    public override IEnumerator<T> GetEnumerator()
    {
        return InOrderTraversal((RedBlackTreeNode?) Root).GetEnumerator();
    }

    /// <summary>
    /// Order traversal of the tree
    /// </summary>
    /// <param name="node">Node from where to start</param>
    /// <returns>The current values of traversed nodes</returns>
    private IEnumerable<T> InOrderTraversal(RedBlackTreeNode? node)
    {
        if (node is null) yield break;
        foreach (var item in InOrderTraversal((RedBlackTreeNode?) node.Right)) yield return item;
        yield return node.Value;
        foreach (var item in InOrderTraversal((RedBlackTreeNode?) node.Left)) yield return item;
    }

    /// <inheritdoc/>
    public override IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return InOrderTraversalAsync((RedBlackTreeNode?) Root, cancellationToken).GetAsyncEnumerator(cancellationToken);
    }

    /// <summary>
    /// Asynchronously traverses the tree in-order
    /// </summary>
    private static async IAsyncEnumerable<T> InOrderTraversalAsync(RedBlackTreeNode? node,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (node is null) yield break;
        await foreach (var item in InOrderTraversalAsync((RedBlackTreeNode?) node.Right, cancellationToken))
            yield return item;
        yield return node.Value;
        await foreach (var item in InOrderTraversalAsync((RedBlackTreeNode?) node.Left, cancellationToken))
            yield return item;
    }
}

/// <summary>
/// Red-black tree JSON converter
/// </summary>
/// <typeparam name="T">Stored type inside the tree</typeparam>
public class RedBlackTreeJsonConverter<T> : JsonConverter<RedBlackTree<T>> where T : IComparable<T>
{
    /// <inheritdoc/>
    public override RedBlackTree<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tree = new RedBlackTree<T>();
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
                                    T value = default!;

                                    while (reader.Read())
                                    {
                                        if (reader.TokenType == JsonTokenType.EndObject) break;

                                        if (reader.TokenType != JsonTokenType.PropertyName) continue;
                                        var innerPropertyName = reader.GetString();
                                        reader.Read(); // Passer à la valeur

                                        value = innerPropertyName switch
                                        {
                                            "Value" => JsonSerializer.Deserialize<T>(ref reader, options) ??
                                                       throw new InvalidOperationException("Value is null"),
                                            _ => value
                                        };
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
public class RedBlackTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    /// <summary>
    /// Node class for Red-Black Tree
    /// </summary>
    private class RedBlackTreeNode(TKey key, TValue value, NodeColor nodeColor = NodeColor.Red)
        : BinarySearchTreeNode(key, value)
    {
        /// <summary>
        /// Color of the node
        /// </summary>
        public NodeColor Color { get; set; } = nodeColor;

        /// <summary>
        /// Node's children and parent
        /// </summary>
        public RedBlackTreeNode? Parent { get; set; }
    }

    /// <summary>
    /// Node color for the red-black tree
    /// </summary>
    private enum NodeColor
    {
        Red = 0xF00,
        Black = 0xFFF
    }

    /// <summary>
    /// Default constructor
    /// </summary>
    public RedBlackTree()
    {
    }

    /// <summary>
    /// Get or set the value of the tree
    /// </summary>
    public override TValue this[TKey key]
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
    public override ICollection<TKey> Keys => this.Select(kvp => kvp.Key).ToList();

    /// <summary>
    /// Values of the tree
    /// </summary>
    public override ICollection<TValue> Values => this.Select(kvp => kvp.Value).ToList();

    /// <inheritdoc/>
    public override void Add(TKey key, TValue value)
    {
        if (Root is null)
        {
            Root = new RedBlackTreeNode(key, value) {Color = NodeColor.Black};
        }
        else
        {
            if (ContainsKey(key)) throw new InvalidOperationException("Key already exists");
            Add((RedBlackTreeNode?) Root, key, value);
        }

        _count++;
    }

    /// <inheritdoc/>
    public override bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }

    /// <summary>
    /// Internal add function helper
    /// </summary>
    private void Add(RedBlackTreeNode? node, TKey key, TValue value)
    {
        if (node is null) return;
        var comparison = node.Key.CompareTo(key);
        if (comparison < 0)
        {
            if (node?.Left is null)
            {
                if (node is not null)
                {
                    node.Left = new RedBlackTreeNode(key, value)
                    {
                        Parent = node
                    };
                    FixTree((RedBlackTreeNode?) node.Left);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add((RedBlackTreeNode?) node.Left, key, value);
            }
        }
        else
        {
            if (node?.Right is null)
            {
                if (node is not null)
                {
                    node.Right = new RedBlackTreeNode(key, value)
                    {
                        Parent = node
                    };
                    FixTree((RedBlackTreeNode?) node.Right);
                }
                else
                {
                    throw new InvalidOperationException("Node is null");
                }
            }
            else
            {
                Add((RedBlackTreeNode?) node.Right, key, value);
            }
        }
    }

    /// <summary>
    /// Fix the tree after adding a node
    /// </summary>
    private void FixTree(RedBlackTreeNode? node)
    {
        while (node?.Parent is not null && node != Root && node.Parent.Color.Equals(NodeColor.Red))
            if (node.Parent == node.Parent.Parent?.Left)
            {
                var uncle = node.Parent.Parent.Right;
                if (uncle is not null && ((RedBlackTreeNode) uncle).Color.Equals(NodeColor.Red))
                {
                    node.Parent.Color = NodeColor.Black;
                    ((RedBlackTreeNode) uncle).Color = NodeColor.Black;
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
                if (uncle is not null && ((RedBlackTreeNode) uncle).Color.Equals(NodeColor.Red))
                {
                    node.Parent.Color = NodeColor.Black;
                    ((RedBlackTreeNode) uncle).Color = NodeColor.Black;
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

        if (Root is not null) ((RedBlackTreeNode) Root).Color = NodeColor.Black;
        else throw new InvalidOperationException("Root is null");
    }

    /// <summary>
    /// Rotate the tree to the left
    /// </summary>
    /// <param name="node">Node from where to start the rotation</param>
    private void RotateLeft(RedBlackTreeNode? node)
    {
        var temp = node?.Right;
        if (node is not null && temp is not null)
        {
            node.Right = temp.Left;
            if (temp.Left is not null) ((RedBlackTreeNode) temp.Left).Parent = node;

            ((RedBlackTreeNode) temp).Parent = node.Parent;
            if (node.Parent is null)
                Root = temp;
            else if (node == node.Parent.Left)
                node.Parent.Left = temp;
            else
                node.Parent.Right = temp;

            temp.Left = node;
            node.Parent = (RedBlackTreeNode?) temp;
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
    private void RotateRight(RedBlackTreeNode? node)
    {
        var temp = node?.Left;
        if (node is not null && temp is not null)
        {
            node.Left = temp.Right;
            if (temp.Right is not null) ((RedBlackTreeNode) temp.Right).Parent = node;

            ((RedBlackTreeNode) temp).Parent = node.Parent;
            if (node.Parent is null)
                Root = temp;
            else if (node == node.Parent.Right)
                node.Parent.Right = temp;
            else
                node.Parent.Left = temp;

            temp.Right = node;
            node.Parent = (RedBlackTreeNode?) temp;
        }
        else
        {
            throw new InvalidOperationException("Node or temp is null");
        }
    }

    /// <inheritdoc/>
    public override bool Remove(TKey key)
    {
        var node = FindNode(key);
        if (node is null) return false;

        if (node.Left is not null && node.Right is not null)
        {
            var temp = GetMinimum((RedBlackTreeNode?) node.Right);
            if (temp is null) throw new InvalidOperationException("Temp is null");
            node.Key = temp.Key;
            node.Value = temp.Value;
            node = temp;
        }

        var child = node.Right ?? node.Left;
        if (child is not null)
        {
            ((RedBlackTreeNode) child).Parent = node.Parent;
            if (node.Parent is null)
                Root = child;
            else if (node == node.Parent.Left)
                node.Parent.Left = child;
            else
                node.Parent.Right = child;
        }
        else if (node.Parent is null)
        {
            Root = null;
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
    private RedBlackTreeNode? FindNode(TKey key)
    {
        var current = Root;
        while (current is not null)
        {
            var comparison = current.Key.CompareTo(key);

            if (comparison < 0)
                current = current.Left;
            else if (comparison > 0)
                current = current.Right;
            else
                return (RedBlackTreeNode?) current;
        }

        return null;
    }

    /// <summary>
    /// Get the minimum node in the tree
    /// </summary>
    private static RedBlackTreeNode? GetMinimum(RedBlackTreeNode? node)
    {
        while (node?.Left is not null) node = (RedBlackTreeNode?) node.Left;
        return node;
    }

    /// <inheritdoc/>
    public override void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public override bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return FindNode(item.Key) is not null;
    }

    /// <inheritdoc/>
    public override bool ContainsKey(TKey key)
    {
        return FindNode(key) is not null;
    }

    /// <inheritdoc/>
    public override bool TryGetValue(TKey key, out TValue value)
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
    public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

        foreach (var item in this) array[arrayIndex++] = item;
    }

    /// <inheritdoc/>
    public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal((RedBlackTreeNode?) Root).GetEnumerator();
    }

    /// <summary>
    /// In order traversal of the tree
    /// </summary>
    private IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(RedBlackTreeNode? node)
    {
        if (node is null) yield break;
        foreach (var item in InOrderTraversal((RedBlackTreeNode?) node.Right)) yield return item;
        yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        foreach (var item in InOrderTraversal((RedBlackTreeNode?) node.Left)) yield return item;
    }

    /// <inheritdoc/>
    public override IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        return InOrderTraversalAsync((RedBlackTreeNode?) Root, cancellationToken).GetAsyncEnumerator(cancellationToken);
    }

    /// <summary>
    /// Asynchronously traverses the tree in-order
    /// </summary>
    private static async IAsyncEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversalAsync(RedBlackTreeNode? node,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (node is null) yield break;
        await foreach (var item in InOrderTraversalAsync((RedBlackTreeNode?) node.Right, cancellationToken))
            yield return item;
        yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        await foreach (var item in InOrderTraversalAsync((RedBlackTreeNode?) node.Left, cancellationToken))
            yield return item;
    }
}

/// <summary>
///  Red-black tree map JSON converter
/// </summary>
public class RedBlackTreeJsonConverter<TKey, TValue> : JsonConverter<RedBlackTree<TKey, TValue>>
    where TKey : IComparable<TKey>
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
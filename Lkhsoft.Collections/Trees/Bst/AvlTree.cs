#region

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace Lkhsoft.Collections.Trees.Bst;

/// <summary>
/// AVL tree implementation
/// </summary>
[JsonConverter(typeof(AvlTreeJsonConverterFactory))]
public class AvlTree<T> : BinarySearchTree<T> where T : IComparable<T>
{
    /// <summary>
    /// Node of the AVL tree
    /// </summary>
    private class AvlTreeNode : BinarySearchTreeNode
    {
        /// <summary>
        /// Height of the node
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Base constructor
        /// </summary>
        public AvlTreeNode(T value) : base(value)
        {
            Height = 1;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    public override void Add(T item)
    {
        Root = Add((AvlTreeNode?) Root, item);
        _count++;
    }

    /// <summary>
    /// Adds the given item to the given subtree
    /// </summary>
    private AvlTreeNode Add(AvlTreeNode? node, T item)
    {
        if (node is null) return new AvlTreeNode(item);

        var comparison = item.CompareTo(node.Value);
        switch (comparison)
        {
            case < 0:
                node.Left = Add((AvlTreeNode?) node.Left, item);
                break;
            case > 0:
                node.Right = Add((AvlTreeNode?) node.Right, item);
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
    public override bool Remove(T item)
    {
        if (!Contains(item)) return false;
        Root = Remove((AvlTreeNode?) Root, item);
        _count--;
        return true;
    }

    /// <summary>
    /// Removes the node with the given value from the given subtree
    /// </summary>
    private AvlTreeNode? Remove(AvlTreeNode? node, T item)
    {
        if (node is null) return null;

        var comparison = item.CompareTo(node.Value);
        switch (comparison)
        {
            case < 0:
                node.Left = Remove((AvlTreeNode?) node.Left, item);
                break;
            case > 0:
                node.Right = Remove((AvlTreeNode?) node.Right, item);
                break;
            default:
            {
                if (node.Left is null) return (AvlTreeNode?) node.Right;
                if (node.Right is null) return (AvlTreeNode?) node.Left;

                var minLargerNode = GetMinimum((AvlTreeNode) node.Right);
                if (minLargerNode is not null)
                {
                    node.Value = minLargerNode.Value;
                    node.Right = Remove((AvlTreeNode?) node.Right, minLargerNode.Value);
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

    /// <summary>
    /// Gets a value indicating whether the tree contains the given item
    /// </summary>
    public override bool Contains(T item)
    {
        return FindNode((AvlTreeNode?) Root, item) is not null;
    }

    /// <summary>
    /// Finds the node with the given value in the given subtree
    /// </summary>
    private AvlTreeNode? FindNode(AvlTreeNode? node, T item)
    {
        while (node is not null)
        {
            var comparison = item.CompareTo(node.Value);
            switch (comparison)
            {
                case < 0:
                    node = (AvlTreeNode?) node.Left;
                    break;
                case > 0:
                    node = (AvlTreeNode?) node.Right;
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
    private AvlTreeNode? GetMinimum(AvlTreeNode avlTreeNode)
    {
        while (avlTreeNode.Left is not null)
            avlTreeNode = (AvlTreeNode) avlTreeNode.Left;
        return avlTreeNode;
    }

    /// <summary>
    /// Updates the height of the given node
    /// </summary>
    private void UpdateHeight(AvlTreeNode avlTreeNode)
    {
        avlTreeNode.Height = 1 + Math.Max(GetHeight((AvlTreeNode?) avlTreeNode.Left),
            GetHeight((AvlTreeNode?) avlTreeNode.Right));
    }

    /// <summary>
    /// Gets the height of the given node
    /// </summary>
    private int GetHeight(AvlTreeNode? node)
    {
        return node?.Height ?? 0;
    }

    /// <summary>
    ///  Gets the balance of the given node
    /// </summary>
    private int GetBalance(AvlTreeNode avlTreeNode)
    {
        return GetHeight((AvlTreeNode?) avlTreeNode.Left) - GetHeight((AvlTreeNode?) avlTreeNode.Right);
    }

    /// <summary>
    /// Balances the given node
    /// </summary>
    private AvlTreeNode Balance(AvlTreeNode avlTreeNode)
    {
        var balance = GetBalance(avlTreeNode);

        switch (balance)
        {
            case > 1:
            {
                if (GetBalance((AvlTreeNode?) avlTreeNode.Left ??
                               throw new InvalidOperationException("Error while balancing left node")) <
                    0)
                    avlTreeNode.Left = RotateLeft((AvlTreeNode) avlTreeNode.Left);
                return RotateRight(avlTreeNode);
            }
            case < -1:
            {
                if (GetBalance((AvlTreeNode) avlTreeNode.Right!) > 0)
                    avlTreeNode.Right = RotateRight((AvlTreeNode?) avlTreeNode.Right ??
                                                    throw new InvalidOperationException(
                                                        "Error while balancing right node"));
                return RotateLeft(avlTreeNode);
            }
            default:
                return avlTreeNode;
        }
    }

    /// <summary>
    ///  Rotates the given node to the left
    /// </summary>
    private AvlTreeNode RotateLeft(AvlTreeNode avlTreeNode)
    {
        var newRoot = avlTreeNode.Right ?? throw new InvalidOperationException("Error while rotating left node");

        avlTreeNode.Right = newRoot.Left;
        newRoot.Left = avlTreeNode;
        UpdateHeight(avlTreeNode);
        UpdateHeight((AvlTreeNode) newRoot);
        return (AvlTreeNode) newRoot;
    }

    /// <summary>
    ///   Rotates the given node to the right
    /// </summary>
    private AvlTreeNode RotateRight(AvlTreeNode avlTreeNode)
    {
        var newRoot = avlTreeNode.Left ?? throw new InvalidOperationException("Error while rotating right node");

        avlTreeNode.Left = newRoot.Right;
        newRoot.Right = avlTreeNode;
        UpdateHeight(avlTreeNode);
        UpdateHeight((AvlTreeNode) newRoot);
        return (AvlTreeNode) newRoot;
    }

    /// <summary>
    /// Copies the elements of the tree to an array, starting at a particular array index
    /// </summary>
    public override void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex > array.Length)
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count)
            throw new ArgumentException("The target array is too small.");

        foreach (var item in this) array[arrayIndex++] = item;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection
    /// </summary>
    public override IEnumerator<T> GetEnumerator()
    {
        return InOrderTraversal((AvlTreeNode?) Root).GetEnumerator();
    }

    /// <summary>
    /// Traverses the tree in in-order
    /// </summary>
    private IEnumerable<T> InOrderTraversal(AvlTreeNode? node)
    {
        if (node is null) yield break;
        foreach (var item in InOrderTraversal((AvlTreeNode?) node.Left))
            yield return item;
        yield return node.Value;
        foreach (var item in InOrderTraversal((AvlTreeNode?) node.Right))
            yield return item;
    }

    /// <inheritdoc/>
    public override IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return InOrderTraversalAsync((AvlTreeNode?) Root, cancellationToken).GetAsyncEnumerator(cancellationToken);
    }

    /// <summary>
    /// Asynchronously traverses the tree in-order
    /// </summary>
    private static async IAsyncEnumerable<T> InOrderTraversalAsync(AvlTreeNode? node,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (node is null) yield break;
        await foreach (var item in InOrderTraversalAsync((AvlTreeNode?) node.Left, cancellationToken))
            yield return item;
        yield return node.Value;
        await foreach (var item in InOrderTraversalAsync((AvlTreeNode?) node.Right, cancellationToken))
            yield return item;
    }
}

/// <summary>
/// AVL tree JSON converter
/// </summary>
/// <typeparam name="T">Stored type inside the tree</typeparam>
public class AvlTreeJsonConverter<T> : JsonConverter<AvlTree<T>> where T : IComparable<T>
{
    /// <inheritdoc/>
    public override AvlTree<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var tree = new AvlTree<T>();
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
    public override void Write(Utf8JsonWriter writer, AvlTree<T> value, JsonSerializerOptions options)
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
/// AVL tree JSON converter factory
/// </summary>
public class AvlTreeJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeof(AvlTree<>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition())) return false;

        var itemType = typeToConvert.GetGenericArguments()[0];
        return typeof(IComparable<>).MakeGenericType(itemType).IsAssignableFrom(itemType);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(AvlTreeJsonConverter<>).MakeGenericType(itemType);
        return (JsonConverter) Activator.CreateInstance(converterType)! ??
               throw new InvalidOperationException("Converter is null");
    }
}

/// <summary>
/// AVL tree map implementation
/// </summary>
[JsonConverter(typeof(AvlTreeMapJsonConverterFactory))]
public class AvlTree<TKey, TValue> : BinarySearchTree<TKey, TValue>
    where TKey : IComparable<TKey>
{
    private class AvlTreeNode(TKey key, TValue value) : BinarySearchTreeNode(key, value)
    {
        /// <summary>
        /// Height of the node
        /// </summary>
        public int Height { get; set; } = 1;
    }

    /// <summary>
    /// Gets the value associated with the given key
    /// </summary>
    public override TValue this[TKey key]
    {
        get
        {
            var node = FindNode((AvlTreeNode?) Root, key);
            if (node is null) throw new KeyNotFoundException($"Key '{key}' not found.");
            return node.Value;
        }
        set => Root = AddOrUpdate((AvlTreeNode?) Root, key, value);
    }

    /// <summary>
    /// Gets the keys of the tree
    /// </summary>
    public override ICollection<TKey> Keys => GetKeys();

    /// <summary>
    /// Gets the values of the tree
    /// </summary>
    public override ICollection<TValue> Values => GetValues();

    /// <inheritdoc/>
    public override void Add(TKey key, TValue value)
    {
        if (ContainsKey(key)) throw new InvalidOperationException("Key already exists");
        Root = Add((AvlTreeNode?) Root, key, value);
        _count++;
    }

    /// <inheritdoc/>
    public override bool ContainsKey(TKey key)
    {
        return FindNode((AvlTreeNode?) Root, key) is not null;
    }

    /// <inheritdoc/>
    public override bool Remove(TKey key)
    {
        if (!ContainsKey(key)) return false;
        Root = Remove((AvlTreeNode?) Root, key);
        _count--;
        return true;
    }

    /// <inheritdoc/>
    public override bool TryGetValue(TKey key, out TValue value)
    {
        var node = FindNode((AvlTreeNode?) Root, key);
        if (node is not null)
        {
            value = node.Value;
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public override void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    /// <inheritdoc/>
    public override bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
    }

    /// <inheritdoc/>
    public override void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        foreach (var kvp in this)
            array[arrayIndex++] = kvp;
    }

    /// <summary>
    /// Removes the given key-value pair from the tree
    /// </summary>
    public override bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return Contains(item) && Remove(item.Key);
    }

    /// <inheritdoc/>
    public override IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return InOrderTraversal((AvlTreeNode?) Root).GetEnumerator();
    }

    /// <summary>
    /// Adds the given key-value pair to the tree
    /// </summary>
    private AvlTreeNode Add(AvlTreeNode? node, TKey key, TValue value)
    {
        if (node is null) return new AvlTreeNode(key, value);

        var comparison = key.CompareTo(node.Key);
        switch (comparison)
        {
            case < 0:
                node.Left = Add((AvlTreeNode?) node.Left, key, value);
                break;
            case > 0:
                node.Right = Add((AvlTreeNode?) node.Right, key, value);
                break;
            default:
                throw new InvalidOperationException("Key already exists");
        }

        UpdateHeight(node);
        return Balance(node);
    }

    /// <summary>
    /// Adds the given key-value pair to the given subtree
    /// </summary>
    private AvlTreeNode AddOrUpdate(AvlTreeNode? node, TKey key, TValue value)
    {
        if (node is null) return new AvlTreeNode(key, value);

        var comparison = key.CompareTo(node.Key);
        switch (comparison)
        {
            case < 0:
                node.Left = AddOrUpdate((AvlTreeNode?) node.Left, key, value);
                break;
            case > 0:
                node.Right = AddOrUpdate((AvlTreeNode?) node.Right, key, value);
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
    private AvlTreeNode? Remove(AvlTreeNode? node, TKey key)
    {
        if (node is null) return null;

        var comparison = key.CompareTo(node.Key);
        switch (comparison)
        {
            case < 0:
                node.Left = Remove((AvlTreeNode?) node.Left, key);
                break;
            case > 0:
                node.Right = Remove((AvlTreeNode?) node.Right, key);
                break;
            default:
            {
                if (node.Left is null) return (AvlTreeNode?) node.Right;
                if (node.Right is null) return (AvlTreeNode?) node.Left;

                var minLargerNode = GetMinimum((AvlTreeNode) node.Right);
                node.Key = minLargerNode.Key;
                node.Value = minLargerNode.Value;
                node.Right = Remove((AvlTreeNode?) node.Right, minLargerNode.Key);
                break;
            }
        }

        UpdateHeight(node);
        return Balance(node);
    }

    /// <summary>
    /// Finds the node with the given key in the given subtree
    /// </summary>
    private static AvlTreeNode? FindNode(AvlTreeNode? node, TKey key)
    {
        while (node is not null)
        {
            var comparison = key.CompareTo(node.Key);
            switch (comparison)
            {
                case < 0:
                    node = (AvlTreeNode?) node.Left;
                    break;
                case > 0:
                    node = (AvlTreeNode?) node.Right;
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
    private static IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(AvlTreeNode? node)
    {
        if (node is null) yield break;
        foreach (var kvp in InOrderTraversal((AvlTreeNode?) node.Left))
            yield return kvp;

        yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);

        foreach (var kvp in InOrderTraversal((AvlTreeNode?) node.Right))
            yield return kvp;
    }

    /// <inheritdoc/>
    public override IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(
        CancellationToken cancellationToken = default)
    {
        return InOrderTraversalAsync((AvlTreeNode?) Root, cancellationToken).GetAsyncEnumerator(cancellationToken);
    }

    /// <summary>
    /// Asynchronously traverses the tree in-order
    /// </summary>
    private static async IAsyncEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversalAsync(AvlTreeNode? node,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (node is null) yield break;
        await foreach (var item in InOrderTraversalAsync((AvlTreeNode?) node.Left, cancellationToken))
            yield return item;
        yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
        await foreach (var item in InOrderTraversalAsync((AvlTreeNode?) node.Right, cancellationToken))
            yield return item;
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
    private AvlTreeNode GetMinimum(AvlTreeNode avlTreeNode)
    {
        while (avlTreeNode.Left is not null)
            avlTreeNode = (AvlTreeNode) avlTreeNode.Left;
        return avlTreeNode;
    }

    /// <summary>
    /// Updates the height of the given node
    /// </summary>
    private void UpdateHeight(AvlTreeNode avlTreeNode)
    {
        avlTreeNode.Height = 1 + Math.Max(GetHeight((AvlTreeNode?) avlTreeNode.Left),
            GetHeight((AvlTreeNode?) avlTreeNode.Right));
    }

    /// <summary>
    /// Gets the height of the given node
    /// </summary>
    private int GetHeight(AvlTreeNode? node)
    {
        return node?.Height ?? 0;
    }

    /// <summary>
    /// Gets the balance of the given node
    /// </summary>
    private int GetBalance(AvlTreeNode avlTreeNode)
    {
        return GetHeight((AvlTreeNode?) avlTreeNode.Left) - GetHeight((AvlTreeNode?) avlTreeNode.Right);
    }

    /// <summary>
    /// Balances the given node
    /// </summary>
    private AvlTreeNode Balance(AvlTreeNode avlTreeNode)
    {
        var balance = GetBalance(avlTreeNode);

        switch (balance)
        {
            case > 1:
            {
                if (GetBalance((AvlTreeNode?) avlTreeNode.Left!) < 0)
                    avlTreeNode.Left = RotateLeft((AvlTreeNode?) avlTreeNode.Left ??
                                                  throw new InvalidOperationException(
                                                      "Error while balancing left node"));
                return RotateRight(avlTreeNode);
            }
            case < -1:
            {
                if (GetBalance((AvlTreeNode?) avlTreeNode.Right!) > 0)
                    avlTreeNode.Right = RotateRight((AvlTreeNode?) avlTreeNode.Right ??
                                                    throw new InvalidOperationException(
                                                        "Error while balancing right node"));
                return RotateLeft(avlTreeNode);
            }
            default:
                return avlTreeNode;
        }
    }

    /// <summary>
    /// Rotates the given node to the left
    /// </summary>
    private AvlTreeNode RotateLeft(AvlTreeNode avlTreeNode)
    {
        var newRoot = (AvlTreeNode?) avlTreeNode.Right!;
        avlTreeNode.Right = newRoot.Left;
        newRoot.Left = avlTreeNode;
        UpdateHeight(avlTreeNode);
        UpdateHeight(newRoot);
        return newRoot;
    }

    /// <summary>
    /// Rotates the given node to the right
    /// </summary>
    private AvlTreeNode RotateRight(AvlTreeNode avlTreeNode)
    {
        var newRoot = (AvlTreeNode?) avlTreeNode.Left!;
        avlTreeNode.Left = newRoot.Right;
        newRoot.Right = avlTreeNode;
        UpdateHeight(avlTreeNode);
        UpdateHeight(newRoot);
        return newRoot;
    }
}

/// <summary>
///  AVL tree map JSON converter
/// </summary>
public class AvlTreeJsonConverter<TKey, TValue> : JsonConverter<AvlTree<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    /// <inheritdoc/>
    public override AvlTree<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var tree = new AvlTree<TKey, TValue>();
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
    public override void Write(Utf8JsonWriter writer, AvlTree<TKey, TValue> value, JsonSerializerOptions options)
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
///  AVL tree map JSON converter factory
/// </summary>
public class AvlTreeMapJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeof(AvlTree<,>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition())) return false;

        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];
        return typeof(IComparable<>).MakeGenericType(keyType).IsAssignableFrom(keyType);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];
        var converterType = typeof(AvlTreeJsonConverter<,>).MakeGenericType(keyType, valueType);
        return (JsonConverter) Activator.CreateInstance(converterType)! ??
               throw new InvalidOperationException("Converter is null");
    }
}
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Lkhsoft.Collections.Trees.Serialization;

namespace Lkhsoft.Collections.Trees.Bst;


/// <summary>
/// Binary search tree implementation
/// </summary>
[JsonConverter(typeof(BinarySearchTreeJsonConverterFactory))]
public class BinarySearchTree<T> : ICollection<T>,  IAsyncEnumerable<T>, IXmlSerializable where T : IComparable<T>
{
    /// <summary>
    /// Binary search tree node
    /// </summary>
    protected class BinarySearchTreeNode
    {
        /// <summary>
        /// Node value
        /// </summary>
        public T Value { get; set; }
        
        /// <summary>
        /// Left child
        /// </summary>
        public BinarySearchTreeNode? Left { get; set; }
        
        /// <summary>
        /// Right child
        /// </summary>
        public BinarySearchTreeNode? Right { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public BinarySearchTreeNode(T value)
        {
            Value = value;
        }
    }

    /// <summary>
    /// Tree root
    /// </summary>
    private protected BinarySearchTreeNode? Root;
    
    /// <summary>
    /// Number of elements in the tree
    /// </summary>
    private protected int _count;

    /// <inheritdoc/>
    public int Count => _count;
    
    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public virtual void Add(T item)
    {
        if(Contains(item)) throw new InvalidOperationException("Item already exists");
        Root = Add(Root, item);
        _count++;
    }

    /// <summary>
    /// Add an item to the tree
    /// </summary>
    private static BinarySearchTreeNode Add(BinarySearchTreeNode? node, T item)
    {
        if (node is null) return new BinarySearchTreeNode(item);

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
    public virtual bool Remove(T item)
    {
        if (!Contains(item)) return false;
        Root = Remove(Root ?? throw new InvalidOperationException("Root is null"), item);
        _count--;
        return true;
    }

    /// <summary>
    /// Removes an item from the tree
    /// </summary>
    private static BinarySearchTreeNode? Remove(BinarySearchTreeNode? node, T item)
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
    public virtual bool Contains(T item)
    {
        return Contains(Root, item);
    }

    /// <summary>
    /// Checks if the tree contains an item
    /// </summary>
    private static bool Contains(BinarySearchTreeNode? node, T item)
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
    public virtual void Clear()
    {
        Root = null;
        _count = 0;
    }

    /// <inheritdoc/>
    public virtual void CopyTo(T[] array, int arrayIndex)
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
    public virtual IEnumerator<T> GetEnumerator()
    {
        return InOrderTraversal(Root ?? throw new InvalidOperationException("Root is null")).GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// In-order traversal of the tree
    /// </summary>
    private static IEnumerable<T> InOrderTraversal(BinarySearchTreeNode? node)
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
    private static BinarySearchTreeNode GetMinimum(BinarySearchTreeNode binarySearchTreeNode)
    {
        while (binarySearchTreeNode.Left != null)
            binarySearchTreeNode = binarySearchTreeNode.Left;
        return binarySearchTreeNode;
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

        // Désérialiser l'arbre
        var serializer = new XmlSerializer(typeof(SerializedNodes<T>));
        var nodes = (SerializedNodes<T>)serializer.Deserialize(reader)!;

        // Ajouter les valeurs à la collection
        nodes?.Nodes.ForEach(x => Add(x.Value));

        // Valider la correspondance du count
        if (nodes?.Count != Count)
            throw new InvalidOperationException("Error while reading the BST tree from XML");
    }

    /// <inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
        var nodes = new List<SerializedNode<T>>(this.Count);
        nodes.AddRange(this.Select(element => new SerializedNode<T>(element)));
        var xmlNodes = new SerializedNodes<T>(nodes);
        new XmlSerializer(typeof(SerializedNodes<T>)).Serialize(writer, xmlNodes);
    }
    
    /// <inheritdoc/>
    public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return InOrderTraversalAsync(Root, cancellationToken).GetAsyncEnumerator(cancellationToken);
    }

    /// <summary>
    /// Asynchronously traverses the tree in-order
    /// </summary>
    private static async IAsyncEnumerable<T> InOrderTraversalAsync(BinarySearchTreeNode? node, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (node is null) yield break;
        await foreach (var item in InOrderTraversalAsync(node.Left, cancellationToken))
            yield return item;
        yield return node.Value;
        await foreach (var item in InOrderTraversalAsync(node.Right, cancellationToken))
            yield return item;
    }
}

/// <summary>
/// BST JSON converter
/// </summary>
public class BinarySearchTreeJsonConverter<T> : JsonConverter<BinarySearchTree<T>> where T : IComparable<T>
{
    /// <inheritdoc/>
    public override BinarySearchTree<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
          var tree = new BinarySearchTree<T>();
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
    public override void Write(Utf8JsonWriter writer, BinarySearchTree<T> value, JsonSerializerOptions options)
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
/// BST JSON converter factory
/// </summary>
public class BinarySearchTreeJsonConverterFactory : JsonConverterFactory 
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeof(BinarySearchTree<>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition())) return false;

        var itemType = typeToConvert.GetGenericArguments()[0];
        return typeof(IComparable<>).MakeGenericType(itemType).IsAssignableFrom(itemType);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(BinarySearchTreeJsonConverter<>).MakeGenericType(itemType);
        return (JsonConverter) Activator.CreateInstance(converterType)! ??
               throw new InvalidOperationException("Converter is null");
    }
}

/// <summary>
/// Binary search tree map implementation
/// </summary>
[JsonConverter(typeof(BinarySearchTreeMapJsonConverterFactory))]
public class BinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue>, IXmlSerializable, IAsyncEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Binary search tree node
        /// </summary>
        protected class BinarySearchTreeNode(TKey key, TValue value)
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
            public BinarySearchTreeNode? Left { get; set; }
            
            /// <summary>
            /// Node right child
            /// </summary>
            public BinarySearchTreeNode? Right { get; set; }
        }

        /// <summary>
        /// Tree root
        /// </summary>
        private protected BinarySearchTreeNode? Root;
        
        /// <summary>
        /// Number of elements in the tree
        /// </summary>
        private protected int _count;

        /// <inheritdoc/>
        public int Count => _count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public virtual TValue this[TKey key]
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
                    Root = UpdateValue(Root, key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <inheritdoc/>
        public virtual ICollection<TKey> Keys
        {
            get
            {
                var keys = new List<TKey>();
                InOrderTraversal(Root, (node) => keys.Add(node.Key));
                return keys;
            }
        }

        /// <inheritdoc/>
        public virtual ICollection<TValue> Values
        {
            get
            {
                var values = new List<TValue>();
                InOrderTraversal(Root, (node) => values.Add(node.Value));
                return values;
            }
        }

        /// <inheritdoc/>
        public virtual void Add(TKey key, TValue value)
        {
            if(ContainsKey(key)) throw new InvalidOperationException("Key already exists");
            Root = Add(Root, key, value);
            _count++;
        }

        /// <summary>
        /// Add a key-value pair to the tree
        /// </summary>
        private static BinarySearchTreeNode Add(BinarySearchTreeNode? node, TKey key, TValue value)
        {
            if (node is null) return new BinarySearchTreeNode(key, value);

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
        public virtual bool Remove(TKey key)
        {
            if (!ContainsKey(key)) return false;
            Root = Remove(Root, key);
            _count--;
            return true;
        }

        /// <summary>
        /// Removes a key from the tree
        /// </summary>
        private static BinarySearchTreeNode? Remove(BinarySearchTreeNode? node, TKey key)
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
        public virtual bool ContainsKey(TKey key)
        {
            return ContainsKey(Root, key);
        }

        /// <summary>
        /// Recursively checks if the tree contains a key
        /// </summary>
        private static bool ContainsKey(BinarySearchTreeNode? node, TKey key)
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
        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            var node = FindNode(Root, key);
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
        private static BinarySearchTreeNode? FindNode(BinarySearchTreeNode? node, TKey key)
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
        public virtual void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public virtual bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            Root = null;
            _count = 0;
        }

        /// <inheritdoc/>
        public virtual bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return TryGetValue(item.Key, out var value) && EqualityComparer<TValue>.Default.Equals(value, item.Value);
        }

        /// <inheritdoc/>
        public virtual void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

            int index = arrayIndex;
            InOrderTraversal(Root, (node) => array[index++] = new KeyValuePair<TKey, TValue>(node.Key, node.Value));
        }

        /// <inheritdoc/>
        public virtual IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return InOrderTraversal(Root).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// In-order traversal of the tree
        /// </summary>
        private static IEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversal(BinarySearchTreeNode? node)
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
        private static void InOrderTraversal(BinarySearchTreeNode? node, Action<BinarySearchTreeNode> action)
        {
            if (node is null) return;
            InOrderTraversal(node.Left, action);
            action(node);
            InOrderTraversal(node.Right, action);
        }

        /// <summary>
        /// Gets the minimum node in the tree
        /// </summary>
        private static BinarySearchTreeNode GetMinimum(BinarySearchTreeNode binarySearchTreeNode)
        {
            while (binarySearchTreeNode.Left is not null)
                binarySearchTreeNode = binarySearchTreeNode.Left;
            return binarySearchTreeNode;
        }

        /// <summary>
        /// Updates the value of a key in the tree
        /// </summary>
        private static BinarySearchTreeNode? UpdateValue(BinarySearchTreeNode? node, TKey key, TValue value)
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

            // Désérialiser l'arbre
            var serializer = new XmlSerializer(typeof(SerializedNodes<TKey, TValue>));
            var nodes = (SerializedNodes<TKey, TValue>)serializer.Deserialize(reader)!;

            // Ajouter les valeurs à la collection
            nodes?.Nodes.ForEach(x => Add(x.Key, x.Value));

            // Valider la correspondance du count
            if (nodes?.Count != Count)
                throw new InvalidOperationException("Error while reading the BST tree from XML");
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer)
        {
            var nodes = new List<SerializedNode<TKey, TValue>>(this.Count);
            nodes.AddRange(this.Select(element => new SerializedNode<TKey, TValue>(element.Key, element.Value)));
            var xmlNodes = new SerializedNodes<TKey, TValue>(nodes);
            new XmlSerializer(typeof(SerializedNodes<TKey, TValue>)).Serialize(writer, xmlNodes);
        }

        /// <inheritdoc/>
        public virtual IAsyncEnumerator<KeyValuePair<TKey, TValue>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return InOrderTraversalAsync(Root, cancellationToken).GetAsyncEnumerator(cancellationToken);
        }

        /// <summary>
        /// Asynchronously traverses the tree in-order
        /// </summary>
        private static async IAsyncEnumerable<KeyValuePair<TKey, TValue>> InOrderTraversalAsync(BinarySearchTreeNode? node, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (node is null) yield break;
            await foreach (var item in InOrderTraversalAsync(node.Left, cancellationToken))
                yield return item;
            yield return new KeyValuePair<TKey, TValue>(node.Key, node.Value);
            await foreach (var item in InOrderTraversalAsync(node.Right, cancellationToken))
                yield return item;
        }
    }
    
    /// <summary>
///  BST map JSON converter
/// </summary>
public class BinarySearchTreeJsonConverter<TKey, TValue> : JsonConverter<BinarySearchTree<TKey, TValue>>
    where TKey : IComparable<TKey>
{
    /// <inheritdoc/>
    public override BinarySearchTree<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        var tree = new BinarySearchTree<TKey, TValue>();
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
    public override void Write(Utf8JsonWriter writer, BinarySearchTree<TKey, TValue> value, JsonSerializerOptions options)
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
///  BST map JSON converter factory
/// </summary>
public class BinarySearchTreeMapJsonConverterFactory : JsonConverterFactory 
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeof(BinarySearchTree<,>).IsAssignableFrom(typeToConvert.GetGenericTypeDefinition())) return false;

        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];
        return typeof(IComparable<>).MakeGenericType(keyType).IsAssignableFrom(keyType);
    }

    /// <inheritdoc/>
    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];
        var converterType = typeof(BinarySearchTreeJsonConverter<,>).MakeGenericType(keyType, valueType);
        return (JsonConverter) Activator.CreateInstance(converterType)! ??
               throw new InvalidOperationException("Converter is null");
    }
}
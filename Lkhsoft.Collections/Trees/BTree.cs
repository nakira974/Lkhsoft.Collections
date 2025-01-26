using System.Collections;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Lkhsoft.Collections.Trees.BTrees;

/// <summary>
/// BTree implementation
/// </summary>
public class BTree<TKey, TValue>(byte containerSize = 3)
    : IDictionary<TKey, IEnumerable<TValue>>, IXmlSerializable, IAsyncEnumerable<KeyValuePair<TKey, IEnumerable<TValue>>>
    where TKey : IComparable<TKey>
    where TValue : IComparable<TValue>
{
    /// <summary>
    /// Btree node
    /// </summary>
    private class Node(TKey key, bool isLeaf)
    {
        /// <summary>
        /// Key of the node
        /// </summary>
        public TKey Key { get; set; } = key;

        /// <summary>
        /// Values of the node
        /// </summary>
        public HashSet<TValue> Values { get; set; } = [];

        /// <summary>
        /// Children of the node
        /// </summary>
        public List<Node> Children { get; set; } = [];

        /// <summary>
        /// Is the node a leaf ?
        /// </summary>
        public bool IsLeaf { get; set; } = isLeaf;

        /// <summary>
        /// Parent of the node
        /// </summary>
        public Node? Parent { get; set; }
    }

    /// <summary>
    /// Tree root
    /// </summary>
    private Node? _root;

    /// <summary>
    /// Number of elements in the tree
    /// </summary>
    private int _count;

    ///<inheritdoc/>
    public int Count => _count;
    
    /// <summary>
    /// Number of nodes in the tree
    /// </summary>
    public int NodesCount => Values.Count;

    ///<inheritdoc/>
    public bool IsReadOnly => false;
    
    ///<inheritdoc/>
    public ICollection<TKey> Keys => this.Select(kvp => kvp.Key).Distinct().ToList();
    
    ///<inheritdoc/>
    public ICollection<IEnumerable<TValue>> Values => this.Select(kvp => kvp.Value).ToList();
    
    /// <summary>
    /// Dictionary to track unique values for each index
    /// </summary>
    private readonly Dictionary<TKey, HashSet<TValue>> _uniqueValues = new();

    ///<inheritdoc/>
    public IEnumerable<TValue> this[TKey key]
    {
        get => GetValues(key);
        set => Insert(key, value);
    }

    /// <summary>
    /// Gets or sets the value at the specified index
    /// </summary>
    /// <param name="key">Key to find the value</param>
    /// <param name="index">Index of the value</param>
    public TValue this[TKey key, int index]
    {
        get
        {
            if (!TryGetValue(key, out var values)) throw new KeyNotFoundException();
            return values.ElementAt(index);
        }
        set
        {
            if (!TryGetValue(key, out var values)) throw new KeyNotFoundException();
            var comparables = values.ToArray();
            if (index < 0 || index >= comparables.Length) throw new IndexOutOfRangeException();
            comparables[index] = value;
            Insert(key, comparables);
        }
    }

    /// <summary>
    /// Gets the values for a given key
    /// </summary>
    private IEnumerable<TValue> GetValues(TKey key)
    {
        var values = new List<TValue>();
        GetValuesHelper(_root, key, values);
        return values;
    }

    /// <summary>
    /// Recursively gets the values for a given key
    /// </summary>
    private static void GetValuesHelper(Node? node, TKey key, List<TValue> values)
    {
        if (node is null) return;

        if (key.CompareTo(node.Key) == 0)
        {
            values.AddRange(node.Values);
        }

        foreach (var child in node.Children)
        {
            GetValuesHelper(child, key, values);
        }
    }

    /// <summary>
    /// Inserts a key-value pair into the tree
    /// </summary>
    private void Insert(TKey key, TValue value)
    {
        if (!_uniqueValues.TryGetValue(key, out var uniqueValues))
        {
            uniqueValues = [];
            _uniqueValues[key] = uniqueValues;
        }

        if (!uniqueValues.Add(value))
        {
            throw new InvalidOperationException("Duplicated values are not allowed for a given index");
        }

        if (_root is null)
        {
            _root = new Node(key, true);
            if (!_root.Values.Add(value))
            {
                throw new InvalidOperationException("Duplicated values are not allowed for a given index");
            };
        }
        else
        {
            if (_root.Values.Count == containerSize)
            {
                var newRoot = new Node(key, false);
                newRoot.Children.Add(_root);
                _root.Parent = newRoot;
                SplitChild(newRoot, 0);
                _root = newRoot;
            }
            InsertNonFull(_root, key, value);
        }

        _count++;
    }

    /// <summary>
    /// Inserts a key-values pair into the tree
    /// </summary>
    private void Insert(TKey key, IEnumerable<TValue> values)
    {
        var comparables = values as TValue[] ?? values.ToArray();
        
        foreach (var value in comparables)
        {
            Insert(key, value);
            _count++;
        }
    }

    /// <summary>
    /// Inserts a key-value pair into a non-full node
    /// </summary>
  private void InsertNonFull(Node x, TKey key, TValue value)
{
    // Recherche de l'enfant approprié
    if (x.IsLeaf)
    {
        // Cas 1 : Le nœud est une feuille
        if (x.Key.CompareTo(key) == 0)
        {
            // Si la clé correspond et que la limite de conteneur n'est pas atteinte, on ajoute
            if (x.Values.Count < containerSize)
            {
                if (!x.Values.Add(value))
                {
                    throw new InvalidOperationException("Duplicated values are not allowed for a given index");
                };
            }
            else
            {
                // Sinon, on redistribue la valeur ou créez un nouveau nœud pour cette clé
                RedistributeOrAddNode(x, key, value);
            }
        }
        else
        {
            // Cas où la clé n'existe pas dans ce nœud, on ajoute un nouveau nœud
            var newNode = new Node(key, true) { Parent = x };
            if (!newNode.Values.Add(value))
            {
                throw new InvalidOperationException("Duplicated values are not allowed for a given index");
            };
            // On ajoute le nouveau nœud dans les enfants
            x.Children.Add(newNode); 
        }
    }
    else
    {
        // Cas 2 : Le nœud n'est pas une feuille
        var i = 0;
        while (i < x.Children.Count && key.CompareTo(x.Children[i].Key) > 0)
        {
            i++;
        }

        // On cherche l'enfant approprié et gérez les cas où le nœud est plein
        if (i < x.Children.Count && x.Children[i].Key.CompareTo(key) == 0)
        {
            // Trouvé un enfant avec la même clé
            if (x.Children[i].Values.Count == containerSize)
            {
                // Redistribuer ou diviser si nécessaire
                RedistributeOrAddNode(x.Children[i], key, value);
            }
            else
            {
                InsertNonFull(x.Children[i], key, value);
            }
        }
        else
        {
            // Aucun nœud approprié, insérez un nouveau nœud pour cette clé
            var newChild = new Node(key, true) { Parent = x };
            if (!newChild.Values.Add(value))
            {
                throw new InvalidOperationException("Duplicated values are not allowed for a given index");
            };
            x.Children.Insert(i, newChild); // Insérez au bon emplacement
        }
    }
}

    /// <summary>
    /// Redistributes values or add a node
    /// </summary>
private void RedistributeOrAddNode(Node x, TKey key, TValue value)
{
    if (x.Parent == null) return;
    foreach (var child in x.Parent.Children)
    {
        if (child.Key.CompareTo(key) != 0 || child.Values.Count >= containerSize) continue;
        if(!child.Values.Add(value))
        {
            throw new InvalidOperationException("Duplicated values are not allowed for a given index");
        }
        return;
    }

    // Si aucune redistribution possible, créez un nouveau nœud
    var newNode = new Node(key, x.IsLeaf) {Parent = x.Parent};
    if (!newNode.Values.Add(value))
    {
        throw new InvalidOperationException("Duplicated values are not allowed for a given index");
    }

    // Insérez le nouveau nœud dans la liste des enfants de manière ordonnée
    var index = x.Parent.Children.IndexOf(x) + 1;
    x.Parent.Children.Insert(index, newNode);
}

    /// <summary>
    /// Splits a child node
    /// </summary>
    private void SplitChild(Node? x, int i)
    {
        if(x is null || x.Children.Count <= i) return;
        var y = x.Children[i];
        var z = new Node(y.Key, y.IsLeaf) { Parent = x };
        x.Children.Insert(i + 1, z);

        for (var j = 0; j < containerSize / 2; j++)
        {
            if(!z.Values.Add(y.Values.Last()))
            {
                throw new InvalidOperationException("Duplicated values are not allowed for a given index");
            }
            y.Values.Remove(y.Values.Last());
        }

        if (y.IsLeaf) return;
        {
            for (var j = 0; j < containerSize / 2 + 1; j++)
            {
                z.Children.Add(y.Children[j + containerSize / 2 + 1]);
                y.Children[j + containerSize / 2 + 1].Parent = z;
            }

            y.Children.RemoveRange(containerSize / 2 + 1, containerSize / 2 + 1);
        }
    }

    /// <summary>
    /// Splits a node
    /// </summary>
    private void SplitNode(Node x, TKey key, TValue value)
    {
        var newNode = new Node(key, true) { Parent = x.Parent };
        if (!newNode.Values.Add(value))
        {
            throw new InvalidOperationException("Duplicated values are not allowed for a given index");
        }

        if (x.Parent is not null)
        {
            x.Parent.Children.Add(newNode);
        }
        else
        {
            var newRoot = new Node(key, false);
            newRoot.Children.Add(x);
            newRoot.Children.Add(newNode);
            x.Parent = newRoot;
            newNode.Parent = newRoot;
            _root = newRoot;
        }
    }

    ///<inheritdoc/>
    public bool Remove(TKey key)
    {
        if (!Remove(_root, key)) return false;
        _uniqueValues.Remove(key);
        return true;

    }

    /// <summary>
    /// Removes a node that contains a deleted key from the tree
    /// </summary>
    private bool Remove(Node? x, TKey key)
    {
        if (x is null) return false;

        var nodesToRemove = new List<Node>();
        FindNodesToRemove(x, key, nodesToRemove);

        foreach (var node in nodesToRemove)
        {
            _count -= node.Values.Count;
            node.Values.Clear();
        }

        foreach (var node in nodesToRemove)
        {
            if (node.IsLeaf)
            {
                if (node.Parent is not null)
                {
                    node.Parent.Children.Remove(node);
                }
            }
            else
            {
                MergeOrRedistribute(node);
            }
        }

        // Gérer la racine après suppression
        HandleRootAfterRemoval();

        return nodesToRemove.Count > 0;
    }

    /// <summary>
    /// Handles the root after removal
    /// </summary>
    private void HandleRootAfterRemoval()
    {
        if (_root is null) return;

        switch (_root.Values.Count)
        {
            // Si la racine est vide
            case 0 when _root.Children.Count == 0:
                _root = null; // L'arbre est vide
                break;
            case 0 when _root.Children.Count == 1:
                // Promouvoir le seul enfant de la racine comme nouvelle racine
                _root = _root.Children[0];
                _root.Parent = null;
                break;
        }
    }

    /// <summary>
    /// Finds nodes to remove
    /// </summary>
    private static void FindNodesToRemove(Node node, TKey key, List<Node> nodesToRemove)
    {
        if (node.Key.CompareTo(key) == 0)
        {
            nodesToRemove.Add(node);
        }

        foreach (var child in node.Children)
        {
            FindNodesToRemove(child, key, nodesToRemove);
        }
    }

    /// <summary>
    /// Merges or redistributes nodes
    /// </summary>
    private void MergeOrRedistribute(Node node)
    {
        if (node.Parent is null)
        {
            // Si le nœud est la racine, aucune redistribution n'est nécessaire ici
            return;
        }

        var index = node.Parent.Children.IndexOf(node);
        var leftSibling = index > 0 ? node.Parent.Children[index - 1] : null;
        var rightSibling = index < node.Parent.Children.Count - 1 ? node.Parent.Children[index + 1] : null;

        if (leftSibling is not null && leftSibling.Children.Count >= containerSize)
        {
            BorrowFromPrev(node.Parent, index);
        }
        else if (rightSibling is not null && rightSibling.Children.Count >= containerSize)
        {
            BorrowFromNext(node.Parent, index);
        }
        else
        {
            if (leftSibling is not null)
            {
                Merge(node.Parent, index - 1);
            }
            else if (rightSibling is not null)
            {
                Merge(node.Parent, index);
            }
        }
    }

    /// <summary>
    /// Borrows a node from the previous sibling
    /// </summary>
    private static void BorrowFromPrev(Node x, int i)
    {
        var y = x.Children[i];
        var z = x.Children[i - 1];

        y.Children.Insert(0, z.Children[^1]);
        z.Children.RemoveAt(z.Children.Count - 1);

        y.Key = z.Key;
        y.Values = z.Values;
    }

    /// <summary>
    /// Borrows a node from the next sibling
    /// </summary>
    private static void BorrowFromNext(Node x, int i)
    {
        var y = x.Children[i];
        var z = x.Children[i + 1];

        y.Children.Add(z.Children[0]);
        z.Children.RemoveAt(0);

        y.Key = z.Key;
        y.Values = z.Values;
    }

    /// <summary>
    /// Merges two nodes
    /// </summary>
    private static void Merge(Node x, int i)
    {
        var y = x.Children[i];
        var z = x.Children[i + 1];

        y.Children.AddRange(z.Children);
        y.Key = z.Key;
        y.Values = z.Values;

        x.Children.RemoveAt(i + 1);
    }

    ///<inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, IEnumerable<TValue>>> GetEnumerator()
    {
        var stack = new Stack<Node>();
        stack.Push(_root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();
            yield return new KeyValuePair<TKey, IEnumerable<TValue>>(node.Key, node.Values);

            foreach (var child in node.Children)
            {
                stack.Push(child);
            }
        }
    }

    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    ///<inheritdoc/>
    public async IAsyncEnumerator<KeyValuePair<TKey, IEnumerable<TValue>>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var stack = new Stack<Node>();
        if (_root is not null) stack.Push(_root);

        while (stack.Count > 0)
        {
            var node = stack.Pop();

            await Task.Yield();
            yield return new KeyValuePair<TKey, IEnumerable<TValue>>(node.Key, node.Values);

            foreach (var child in node.Children)
            {
                stack.Push(child);
            }
        }
    }

    ///<inheritdoc/>
    public void Add(TKey key, IEnumerable<TValue> value)
    {
        Insert(key, value);
    }

    /// <summary>
    /// Adds a key-value pair to the tree
    /// </summary>
    public void Add(TKey key, TValue value)
    {
        Insert(key, value);
    }

    ///<inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return GetValues(key).Any();
    }

    /// <summary>
    /// Searches for a value in the tree
    /// </summary>
    /// <returns>The index of the given value</returns>
    public int Search(TKey key, TValue value)
    {
        if (!TryGetValue(key, out var keyValues)) return -1;
        var comparables = keyValues.OrderBy(x => x).ToArray();
        return Array.BinarySearch(comparables, value);
    }

    /// <summary>
    /// Searches for a value in the tree
    /// </summary>
    /// <param name="key">Key of the value to find</param>
    /// <param name="value">Comparable value to find</param>
    /// <param name="foundValue">Found value</param>
    /// <returns>The index of the given value</returns>
    public int Search(TKey key, TValue value, out TValue? foundValue)
    {
        foundValue = default(TValue);
        if (!TryGetValue(key, out var keyValues)) return -1;
        var comparables = keyValues.OrderBy(x => x).ToArray();
        var index = Array.BinarySearch(comparables, value);
        foundValue = comparables[index];
        return index;
    }

    /// <summary>
    /// Searches for a value in the tree
    /// </summary>
    /// <param name="key">Key of the values to find</param>
    /// <param name="values">Values to find</param>
    /// <returns>Index of searched values</returns>
    public IEnumerable<int>? Search(TKey key, IEnumerable<TValue> values)
    {
        var enumerable = values.ToArray();
        if (!TryGetValue(key, out var keyValues) || enumerable.Length == 0) return null;
        var result = new List<int>(enumerable.Length);
        var comparables = keyValues.OrderBy(x => x).ToArray();
        
        foreach (var value in enumerable)
        {
            result.Add(Array.BinarySearch(comparables, value));
        }

        return result;
    }

    /// <summary>
    /// Searches for a value in the tree
    /// </summary>
    /// <param name="key">Key of the values to find</param>
    /// <param name="values">Values to find</param>
    /// <param name="foundValue">Found values</param>
    /// <returns>Index of searched values</returns>
    public IEnumerable<int>? Search(TKey key, IEnumerable<TValue> values, out IEnumerable<TValue>? foundValue)
    {
        foundValue = null;
        var enumerable = values.ToArray();
        if (!TryGetValue(key, out var keyValues) || enumerable.Length == 0) return null;
        var result = new List<int>(enumerable.Length);
        var comparables = keyValues.OrderBy(x => x).ToArray();
       
        foreach (var value in enumerable)
        {
            result.Add(Array.BinarySearch(comparables, value));
        }

        foundValue = result.Select(i => comparables[i]);

        return result;
    }
    
    ///<inheritdoc/>
    public bool TryGetValue(TKey key, out IEnumerable<TValue> value)
    {
        var values = GetValues(key);
        IEnumerable<TValue> comparables = values as TValue[] ?? values.ToArray();
        if (comparables.Any())
        {
            value = comparables;
            return true;
        }
        value = new List<TValue>();
        return false;
    }

    ///<inheritdoc/>
    public void Add(KeyValuePair<TKey, IEnumerable<TValue>> item)
    {
        Insert(item.Key, item.Value);
    }

    ///<inheritdoc/>
    public void Clear()
    {
        _root = null;
        _count = 0;
        _uniqueValues.Clear();
    }

    ///<inheritdoc/>
    public bool Contains(KeyValuePair<TKey, IEnumerable<TValue>> item)
    {
        return TryGetValue(item.Key, out var value) && EqualityComparer<IEnumerable<TValue>>.Default.Equals(value, item.Value);
    }

    ///<inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, IEnumerable<TValue>>[] array, int arrayIndex)
    {
        foreach (var kvp in this)
        {
            array[arrayIndex++] = kvp;
        }
    }

    ///<inheritdoc/>
    public bool Remove(KeyValuePair<TKey, IEnumerable<TValue>> item)
    {
        if (!Contains(item)) return false;
        Remove(item.Key);
        return true;
    }

    ///<inheritdoc/>
    public XmlSchema GetSchema()
    {
        return null;
    }

    ///<inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        throw new NotImplementedException();
    }

    ///<inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
        throw new NotImplementedException();
    }
}

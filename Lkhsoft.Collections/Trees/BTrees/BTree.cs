#region

using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

#endregion

namespace Lkhsoft.Collections.Trees.BTrees;

/// <summary>
/// B-Tree implementation
/// </summary>
public class BTree<T>(byte containerSize = 3) : ICollection<T>, IXmlSerializable, IAsyncEnumerable<T>
    where T : IComparable<T>
{
    /// <summary>
    /// BTree node
    /// </summary>
    private class BTreeNode(bool isLeaf = true)
    {
        /// <summary>
        /// Keys of the node
        /// </summary>
        public List<T> Keys { get; set; } = [];

        /// <summary>
        /// Children of the node
        /// </summary>
        public List<BTreeNode> Children { get; set; } = [];

        /// <summary>
        /// Is the node a leaf ?
        /// </summary>
        public bool IsLeaf { get; set; } = isLeaf;
    }

    /// <summary>
    /// Root of the tree
    /// </summary>
    private BTreeNode? _root;

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
        if (_root is null)
        {
            _root = new BTreeNode {Keys = {item}};
        }
        else
        {
            if (_root.Keys.Count == 2 * containerSize - 1)
            {
                var newRoot = new BTreeNode {IsLeaf = false, Children = {_root}};
                SplitChild(newRoot, 0, _root);
                _root = newRoot;
            }

            InsertNonFull(_root, item);
        }

        _count++;
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (_root is null) return false;
        if (!Remove(_root, item)) return false;
        _count--;
        return true;
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
        return Contains(_root, item);
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        if (arrayIndex < 0 || arrayIndex >= array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Array is too small");

        foreach (var item in this) array[arrayIndex++] = item;
    }

    /// <summary>
    /// Insert the item in the node
    /// </summary>
    private void InsertNonFull(BTreeNode node, T item)
    {
        if (node.IsLeaf)
        {
            var i = node.Keys.Count - 1;
            while (i >= 0 && item.CompareTo(node.Keys[i]) < 0) i--;
            node.Keys.Insert(i + 1, item);
        }
        else
        {
            var i = node.Keys.Count - 1;
            while (i >= 0 && item.CompareTo(node.Keys[i]) < 0) i--;
            i++;
            if (node.Children[i].Keys.Count == 2 * containerSize - 1)
            {
                SplitChild(node, i, node.Children[i]);
                if (item.CompareTo(node.Keys[i]) > 0) i++;
            }

            InsertNonFull(node.Children[i], item);
        }
    }

    /// <summary>
    /// Split the child node
    /// </summary>
    private void SplitChild(BTreeNode parent, int i, BTreeNode child)
    {
        var newChild = new BTreeNode {IsLeaf = child.IsLeaf};
        parent.Children.Insert(i + 1, newChild);
        parent.Keys.Insert(i, child.Keys[containerSize - 1]);

        for (var j = 0; j < containerSize - 1; j++) newChild.Keys.Add(child.Keys[containerSize + j]);

        if (!child.IsLeaf)
            for (var j = 0; j < containerSize; j++)
                newChild.Children.Add(child.Children[containerSize + j]);

        child.Keys.RemoveRange(containerSize - 1, containerSize);
        if (!child.IsLeaf) child.Children.RemoveRange(containerSize, containerSize);
    }

    /// <summary>
    /// Remove the item from the node
    /// </summary>
    private bool Remove(BTreeNode node, T item)
    {
        var i = 0;
        while (i < node.Keys.Count && item.CompareTo(node.Keys[i]) > 0) i++;

        if (i < node.Keys.Count && item.CompareTo(node.Keys[i]) == 0)
        {
            if (node.IsLeaf)
            {
                node.Keys.RemoveAt(i);
                return true;
            }

            if (node.Children[i].Keys.Count >= containerSize)
            {
                var pred = GetPredecessor(node.Children[i]);
                node.Keys[i] = pred;
                return Remove(node.Children[i], pred);
            }

            if (node.Children[i + 1].Keys.Count >= containerSize)
            {
                var succ = GetSuccessor(node.Children[i + 1]);
                node.Keys[i] = succ;
                return Remove(node.Children[i + 1], succ);
            }

            Merge(node, i);
            return Remove(node.Children[i], item);
        }

        if (node.IsLeaf) return false;

        if (node.Children[i].Keys.Count < containerSize) Fill(node, i);
        if (i < node.Keys.Count && item.CompareTo(node.Keys[i]) > 0) i++;
        return Remove(node.Children[i], item);
    }

    /// <summary>
    /// Fill the node
    /// </summary>
    private void Fill(BTreeNode node, int i)
    {
        if (i != 0 && node.Children[i - 1].Keys.Count >= containerSize)
        {
            BorrowFromPrev(node, i);
        }
        else if (i != node.Keys.Count && node.Children[i + 1].Keys.Count >= containerSize)
        {
            BorrowFromNext(node, i);
        }
        else
        {
            if (i != node.Keys.Count)
                Merge(node, i);
            else
                Merge(node, i - 1);
        }
    }

    /// <summary>
    /// Borrow from the previous node
    /// </summary>
    private static void BorrowFromPrev(BTreeNode node, int i)
    {
        var child = node.Children[i];
        var sibling = node.Children[i - 1];

        for (var j = child.Keys.Count - 1; j >= 0; j--) child.Keys[j + 1] = child.Keys[j];

        if (!child.IsLeaf)
            for (var j = child.Children.Count - 1; j >= 0; j--)
                child.Children[j + 1] = child.Children[j];

        child.Keys[0] = node.Keys[i - 1];

        if (!child.IsLeaf) child.Children[0] = sibling.Children[^1];

        node.Keys[i - 1] = sibling.Keys[^1];
        sibling.Keys.RemoveAt(sibling.Keys.Count - 1);

        if (!sibling.IsLeaf) sibling.Children.RemoveAt(sibling.Children.Count - 1);
    }

    /// <summary>
    /// Borrow from the next node
    /// </summary>
    private static void BorrowFromNext(BTreeNode node, int i)
    {
        var child = node.Children[i];
        var sibling = node.Children[i + 1];

        child.Keys.Add(node.Keys[i]);
        node.Keys[i] = sibling.Keys[0];

        if (!child.IsLeaf) child.Children.Add(sibling.Children[0]);

        sibling.Keys.RemoveAt(0);

        if (!sibling.IsLeaf) sibling.Children.RemoveAt(0);
    }

    /// <summary>
    /// Merge the node with its sibling
    /// </summary>
    private static void Merge(BTreeNode node, int i)
    {
        var child = node.Children[i];
        var sibling = node.Children[i + 1];

        child.Keys.Add(node.Keys[i]);
        node.Keys.RemoveAt(i);

        child.Keys.AddRange(sibling.Keys);

        if (!child.IsLeaf) child.Children.AddRange(sibling.Children);

        node.Children.RemoveAt(i + 1);
    }

    /// <summary>
    /// Get the predecessor of the node
    /// </summary>
    private static T GetPredecessor(BTreeNode node)
    {
        while (!node.IsLeaf) node = node.Children[^1];
        return node.Keys[^1];
    }

    /// <summary>
    /// Get the successor of the node
    /// </summary>
    private static T GetSuccessor(BTreeNode node)
    {
        while (!node.IsLeaf) node = node.Children[0];
        return node.Keys[0];
    }

    /// <summary>
    /// Traverse the tree to find if the item is present
    /// </summary>
    private static bool Contains(BTreeNode? node, T item)
    {
        if (node is null) return false;

        var i = 0;
        while (i < node.Keys.Count && item.CompareTo(node.Keys[i]) > 0) i++;

        if (i < node.Keys.Count && item.CompareTo(node.Keys[i]) == 0) return true;

        if (node.IsLeaf) return false;

        return Contains(node.Children[i], item);
    }

    /// <inheritdoc/>
    public XmlSchema? GetSchema()
    {
        return null;
    }

    /// <inheritdoc/>
    public void ReadXml(XmlReader reader)
    {
        Clear();

        var serializer = new XmlSerializer(typeof(List<T>));
        var items = (List<T>) serializer.Deserialize(reader)!;

        foreach (var item in items) Add(item);
    }

    /// <inheritdoc/>
    public void WriteXml(XmlWriter writer)
    {
        var serializer = new XmlSerializer(typeof(List<T>));
        serializer.Serialize(writer, this.ToList());
    }

    /// <summary>
    /// Asynchronous inorder traversal
    /// </summary>
    private static IEnumerable<T> InOrderTraversal(BTreeNode? node)
    {
        if (node is null) yield break;

        for (var i = 0; i < node.Keys.Count; i++)
        {
            if (node.Children.Count > 0)
                foreach (var item in InOrderTraversal(node.Children[i]))
                    yield return item;

            yield return node.Keys[i];
        }

        if (node.Children.Count <= 0) yield break;
        {
            foreach (var item in InOrderTraversal(node.Children[^1])) yield return item;
        }
    }

    /// <summary>
    /// Asynchronous inorder traversal
    /// </summary>
    private static async IAsyncEnumerable<T> InOrderTraversalAsync(BTreeNode? node,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (node is null) yield break;

        for (var i = 0; i < node.Keys.Count; i++)
        {
            if (node.Children.Count > 0)
                await foreach (var item in InOrderTraversalAsync(node.Children[i], cancellationToken))
                    yield return item;

            yield return node.Keys[i];
        }

        if (node.Children.Count <= 0) yield break;
        {
            await foreach (var item in InOrderTraversalAsync(node.Children[^1], cancellationToken)) yield return item;
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

    /// <inheritdoc/>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return InOrderTraversalAsync(_root, cancellationToken).GetAsyncEnumerator(cancellationToken);
    }
}
#region

using System.Collections;

#endregion

namespace Lkhsoft.Collections;

/// <summary>
/// Trie implementation
/// </summary>
public class Trie : ICollection<string>
{
    /// <summary>
    /// Trie node
    /// </summary>
    private class TrieNode
    {
        /// <summary>
        /// Children of the node
        /// </summary>
        public Dictionary<char, TrieNode> Children { get; } = new();

        /// <summary>
        /// Indicates whether the node is the end of a word
        /// </summary>
        public bool IsEndOfWord { get; set; }
    }

    /// <summary>
    /// Root node of the trie
    /// </summary>
    private readonly TrieNode _root = new();

    /// <summary>
    /// Words count in the trie
    /// </summary>
    private int _count;

    /// <inheritdoc/>
    public int Count => _count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public void Add(string item)
    {
        if (string.IsNullOrEmpty(item)) throw new ArgumentException("String cannot be null or empty.", nameof(item));

        var node = _root;
        foreach (var c in item)
        {
            if (!node.Children.ContainsKey(c)) node.Children[c] = new TrieNode();

            node = node.Children[c];
        }

        if (node.IsEndOfWord) return;
        node.IsEndOfWord = true;
        _count++;
    }

    /// <inheritdoc/>
    public bool Contains(string item)
    {
        return !string.IsNullOrEmpty(item) &&
               // Masque alphanumérique et recherche 2DP
               ContainsWithMask(item, _root, 0);
    }

    /// <summary>
    /// Search for a pattern in the trie using LCS approach
    /// </summary>
    private bool ContainsWithMask(string pattern, TrieNode node, int index)
    {
        if (index == pattern.Length) return node.IsEndOfWord;

        var currentChar = pattern[index];

        if (currentChar is '?') // Masque : '?' correspond à n'importe quel caractère
        {
            foreach (var child in node.Children.Values)
                if (ContainsWithMask(pattern, child, index + 1))
                    return true;
        }
        else if (node.Children.TryGetValue(currentChar, out var child))
        {
            return ContainsWithMask(pattern, child, index + 1);
        }

        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _root.Children.Clear();
        _count = 0;
    }

    /// <inheritdoc/>
    public bool Remove(string item)
    {
        return !string.IsNullOrEmpty(item) && Remove(_root, item, 0);
    }

    /// <summary>
    /// Removes the item from the trie.
    /// </summary>
    private bool Remove(TrieNode node, string item, int index)
    {
        if (index == item.Length)
        {
            if (!node.IsEndOfWord) return false;

            node.IsEndOfWord = false;
            _count--;
            return node.Children.Count == 0;
        }

        var currentChar = item[index];
        if (!node.Children.TryGetValue(currentChar, out var childNode)) return false;

        var shouldDeleteChild = Remove(childNode, item, index + 1);

        if (!shouldDeleteChild) return false;
        node.Children.Remove(currentChar);
        return !node.IsEndOfWord && node.Children.Count == 0;
    }

    /// <inheritdoc/>
    public IEnumerator<string> GetEnumerator()
    {
        return Traverse(_root, string.Empty).GetEnumerator();
    }

    /// <summary>
    /// Traverse the trie and yield return all words.
    /// </summary>
    private IEnumerable<string> Traverse(TrieNode node, string prefix)
    {
        if (node.IsEndOfWord) yield return prefix;

        foreach (var kvp in node.Children)
        foreach (var word in Traverse(kvp.Value, prefix + kvp.Key))
            yield return word;
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc/>
    public void CopyTo(string[] array, int arrayIndex)
    {
        if (array is null) throw new ArgumentNullException(nameof(array));
        if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        if (array.Length - arrayIndex < Count) throw new ArgumentException("Insufficient space in target array.");

        foreach (var item in this) array[arrayIndex++] = item;
    }
}
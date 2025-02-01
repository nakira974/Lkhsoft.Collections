using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Lkhsoft.Collections.Trees.BTrees;

namespace Lkhsoft.Collections.Maps;

/// <summary>
/// Sorted multimap implementation
/// </summary>
public class SortedMultiDictionary<TKey, TValue> : IDictionary<TKey, ICollection<TValue>>, IAsyncEnumerable<KeyValuePair<TKey, ICollection<TValue>>>
    where TKey : IComparable<TKey>
    where TValue : IComparable<TValue>
{
    private readonly SortedDictionary<TKey, ICollection<TValue>> _sortedKeys = new();
    
    ///<inheritdoc/>
    public ICollection<TValue> this[TKey key]
    {
        get
        {
            if(!_sortedKeys.TryGetValue(key, out var values))
                throw new KeyNotFoundException();
            return values ?? [];
        }
        set
        {
            if (!_sortedKeys.TryGetValue(key, out var values))
            {
                _sortedKeys.Add(key, new BTree<TValue>(8));
                foreach (var val in value ?? throw new ArgumentNullException())
                {
                    _sortedKeys[key].Add(val);
                }
                return;
            }
            foreach(var val in value ?? throw new ArgumentNullException())
                values.Add(val);
        }
    }

    ///<inheritdoc/>
    public ICollection<TKey> Keys => _sortedKeys.Keys;

    ///<inheritdoc/>
    public ICollection<ICollection<TValue>> Values
    {
        get
        {
            ICollection<ICollection<TValue>> result = new List<ICollection<TValue>>();
            foreach (var kvp in _sortedKeys)
            {
                result.Add(kvp.Value);
            }
            return result;
        }
    }

    ///<inheritdoc/>
    public int Count => _sortedKeys.Count;

    ///<inheritdoc/>
    public bool IsReadOnly => false;

    ///<inheritdoc/>
    public void Add(KeyValuePair<TKey, ICollection<TValue>> item)
    {
        if (!ContainsKey(item.Key))
            _sortedKeys.Add(item.Key, new BTree<TValue>(8));
        
        foreach (var value in item.Value ?? throw new ArgumentNullException())
        {
            _sortedKeys[item.Key].Add(value);
        }
    }

    ///<inheritdoc/>
    public void Clear()
    {
        _sortedKeys.Clear();
    }

    ///<inheritdoc/>
    public bool Contains(KeyValuePair<TKey, ICollection<TValue>> item)
    {
        return _sortedKeys.ContainsKey(item.Key);
    }

    ///<inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, ICollection<TValue>>[] array, int arrayIndex)
    {
        _sortedKeys.CopyTo(array, arrayIndex);
    }
    
    ///<inheritdoc/>
    public void Add(TKey key, ICollection<TValue> value)
    {
        _sortedKeys[key] = value;
    }
    
    /// <summary>
    /// Adds a value to the collection of values associated with the specified key
    /// </summary>
    public void Add(TKey key, TValue value)
    {
        if (!_sortedKeys.TryGetValue(key, out var values))
        {
            _sortedKeys.Add(key, new BTree<TValue>(8));
            _sortedKeys[key].Add(value);
        }
        values?.Add(value);
    }

    ///<inheritdoc/>
    public bool ContainsKey(TKey key)
    {
        return _sortedKeys.ContainsKey(key);
    }

    ///<inheritdoc/>
    public bool Remove(TKey key)
    {
        return _sortedKeys.Remove(key);
    }
    
    ///<inheritdoc/>
    public bool Remove(KeyValuePair<TKey, ICollection<TValue>> item)
    {
        return Remove(item.Key);
    }

    ///<inheritdoc/>
    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out ICollection<TValue> value)
    {
        return _sortedKeys.TryGetValue(key, out value);
    }
    
    ///<inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    
    ///<inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> GetEnumerator()
    {
        foreach (var kvp in _sortedKeys)
        {
            yield return new KeyValuePair<TKey, ICollection<TValue>>(kvp.Key, kvp.Value);
        }
    }

    ///<inheritdoc/>
    public IAsyncEnumerator<KeyValuePair<TKey, ICollection<TValue>>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return new AsyncSortedDictionaryEnumerator(_sortedKeys);
    }
    
    ///<inheritdoc/>
    private class AsyncSortedDictionaryEnumerator(
        SortedDictionary<TKey, ICollection<TValue>> sortedDictionary)
        : IAsyncEnumerator<KeyValuePair<TKey, ICollection<TValue>>>
    {
        /// <summary>
        /// Internal enumerator
        /// </summary>
        private readonly IEnumerator<KeyValuePair<TKey, ICollection<TValue>>> _enumerator = sortedDictionary.GetEnumerator();

        ///<inheritdoc/>
        public KeyValuePair<TKey, ICollection<TValue>> Current => _enumerator.Current;

        ///<inheritdoc/>
        public ValueTask DisposeAsync()
        {
            _enumerator.Dispose();
            return ValueTask.CompletedTask;
        }

        ///<inheritdoc/>
        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_enumerator.MoveNext());
        }
    }
}